;Директивы, посредством которых, регистрам присваиваются понятные имена
.include "m32def.inc"

.def TEMP_REG = r16;регистр для хранения промежуточных данных
.def PRESENCE_REG = r17 ;регистр, который будет хранить (0 << SENSOR_LINE) если обнаружено устройство  
.def SENSOR_DATA_REG = r18
.def SENDING_BYTE_REG = r19; Регистр, хранящий байт для датчика, который собирается отправить микроконтроллер
.def TEMP_REG2 = r20; Ещё один регистр для хранения промежуточных данных
.def SENSOR_DATA_REG2 = r21
.def TEMPERATURE_BYTE_REG = r22
.def TEMPERATURE_BYTE_REG2 = r23

.org 0x0000
 rjmp INIT

.org 0x001a
 call handleCommandByteFromPC
reti
 
.equ SENSOR_LINE = 3
 
;Временные задержки для функиций передачи бит. Рассчитаны на микроконтроллер с частотой 4МГц.
.equ A = 4
.equ B = 37
.equ C = 50
.equ D = 6
.equ E = 10
.equ F = 55
.equ G = 0
.equ H = 300
.equ I = 40
.equ J = 239
.equ U2X_VALUE = 0
.equ UBRR_VALUE = 103

INIT:
;Настройка стека
;После выполнения этих команд,стек будет расти начиная с конца RAM
 ldi TEMP_REG, high(RAMEND)
 out SPH, TEMP_REG
 ldi TEMP_REG, low(RAMEND)
 out SPL, TEMP_REG
 
;PRESENCE_REG будет хранить (0 << SENSOR_LINE), в том случае если обнаружено устройство.
;Поэтому изначально туда внесено значение (1 << SENSOR_LINE). 
 ldi PRESENCE_REG, 1 << SENSOR_LINE

;Инициализация порта на вход.
 ldi TEMP_REG, (0 << SENSOR_LINE); Загрузка (0 << SENSOR_LINE) в TEMP_REG
 out DDRD, TEMP_REG ; Подача (0 << SENSOR_LINE) в регистр DDRD, блгодаря этому выход порта D с номером
;SENSOR_LINE будет сконфигурирован как вход
 out PORTD, TEMP_REG ; Если загрузить (0 << SENSOR_LINE) в DDRD,и в PORTD то нужный вывод будет переведен в третье состояние(Z-state).
 ldi TEMP_REG, 0b00000011
 out DDRC, TEMP_REG
 sei
 call initUSART 
 
 LOOP: rjmp LOOP

HandleCommandByte0x01:
 push TEMP_REG
 ldi TEMP_REG, 0x00
 out PORTC, TEMP_REG
 call findDevice ;Вызов подпрограммы поиска устройства
 ldi SENDING_BYTE_REG, 0x0F;Если устройтсво не найдено, отправляем байт 0x0F на шину 
 sbrs PRESENCE_REG, SENSOR_LINE ; проверка наличия устройства 
 ldi SENDING_BYTE_REG, 0xFF ;Если устройтсво найдено, отправляем байт 0xFF на шину
 sbrs PRESENCE_REG, SENSOR_LINE
 sbi PORTC, 0
 call sendByteToUSART
 pop TEMP_REG
 
rjmp LOOP

HandleCommandByte0x02:
 call skipRom ;Пропускаем идентификацию(поскольку датчик один)
 call convertTemperature ;Начинаем конвертацию температуры
 call findDevice ;Ещё раз пускаем сигнал сброса
 call skipRom ;Ещё раз вызываем SkipRom
 call readData ;Считываем данные
 call findDevice ;После чтения двух байт, прекращаем передачу

 mov SENDING_BYTE_REG, TEMPERATURE_BYTE_REG
 call sendByteToUSART

 mov SENDING_BYTE_REG, TEMPERATURE_BYTE_REG2
 call sendByteToUSART
 
 ldi SENDING_BYTE_REG, 0xFF
 call sendByteToUSART
 
 sbi PORTC, 1
 
rjmp LOOP
 
 
handleCommandByteFromPC:
 cli
 push TEMP_REG
 in TEMP_REG, UDR
 cpi TEMP_REG, 0x01
 breq handleCommandByte0x01
 cpi TEMP_REG, 0x02
 breq handleCommandByte0x02
 pop TEMP_REG
 sei
ret 


initUSART:
 ldi TEMP_REG, high(UBRR_VALUE)
 out UBRRH, TEMP_REG
 ldi TEMP_REG, low(UBRR_VALUE)
 out UBRRL, TEMP_REG
 sbi UCSRB, RXCIE
 sbi UCSRB, RXEN
 sbi UCSRB, TXEN
 ldi TEMP_REG, (U2X_VALUE << 1)
 out UCSRA, TEMP_REG
 ldi TEMP_REG, 0b10001110
 out UCSRC, TEMP_REG
ret

sendByteToUSART:
 in TEMP_REG, UCSRA
 sbrs TEMP_REG, UDRE
 jmp sendByteToUSART
 out UDR, SENDING_BYTE_REG 
ret

skipROM: ;Пропуск ROM
 ldi SENDING_BYTE_REG, 0xCC;Загружаем код команды SkipROM, в специальный регистр
 call writeByte;Вызыов подпрограммы для отправления байта по шине
ret

convertTemperature: ;Подпрограмма конвертации температуры
 ldi SENDING_BYTE_REG, 0x44 ;Готовим к отправке код команды
 call writeByte ;Отправляем байт по шине
 ldi TEMP_REG, 1 ;Костыль, дабы работать с командой cpse 
 
 WAIT_READINESS: ;Ожидаем пока датчик считает температуру
  call readBite ; Считываем бит. В зависимости от полученного значения делаем вывод о готовности датчика к передаче температуры
  cpse SENSOR_DATA_REG, TEMP_REG
  rjmp WAIT_READINESS 

ret

readData:
 ldi SENDING_BYTE_REG, 0xBE
 call writeByte
 call readByte
 mov TEMPERATURE_BYTE_REG, SENSOR_DATA_REG2
 call readByte
 mov TEMPERATURE_BYTE_REG2, SENSOR_DATA_REG2
ret

writeByte: ;подпрограмма отправки байта
 ldi TEMP_REG, 8 ;Значение данного регистра показывает количество оставшихся бит, нужных для отправки

 LOOP_0:  ;Метка для реализации цикла
  cpi TEMP_REG, 0 ;Проверяем, остались ли биты
  breq EXIT_LOOP_0 ;Если битов не осталось, выходим из цикла
  subi TEMP_REG, 1 ;Уменьшаем счетчик бит
  mov TEMP_REG2, SENDING_BYTE_REG;Используем промежуточную переменную
  andi TEMP_REG2, 1 ;Получаем младший бит байта 
  cpi TEMP_REG2, 1 ;Проверяем значение младшего бита
  brne WRITE_ZERO ;Если бит не равен 1, обрабатываем случай отдельно
  call writeBiteOne ;Если бит равен 1, просто вызываем подпрограмму для отправления 1 по шине
  jmp BYTE_SHIFT ;После сдвигаем отправляемый байт вправо

  WRITE_ZERO:;Обработка ситуации, когда отправляемый бит оказался равен 1
   call writeBiteZero ;просто вызываем подпрограмму для отправления 0 по шине 

  BYTE_SHIFT:
   lsr SENDING_BYTE_REG ;Сдвиг отправляемого байта вправо
   jmp LOOP_0
 
 EXIT_LOOP_0:
ret

readByte: ;подпрограмма чтения байта
 ldi TEMP_REG, 8 ;Значение данного регистра показывает количество оставшихся, считываемых бит/ 
 ldi SENSOR_DATA_REG2, 0; Данная переменная будет в конце, содержать итоговый результат чтения
 ldi TEMP_REG2, 0 ;Костыль, созданный дабы было удобнее работать с командой asm cpse

 LOOP_1: ;Метка для зациклиания
  cpi TEMP_REG, 0 ;Проверяем, остались ли биты
  breq EXIT_LOOP_1 ;Если битов не осталось, выходим из цикла
  lsr SENSOR_DATA_REG2 ;Сдвигаем байт вправо
  call readBite ;Вызов подпрограммы чтения бита с шины
  cpse SENSOR_DATA_REG, TEMP_REG2 ;Сверяем считанный бит с 0, если равен, пропустить следующую команду
  sbr SENSOR_DATA_REG2, 0b10000000 ;Устанавилваем старший бит в 1
  subi TEMP_REG, 1 ;Уменьшаем счётчик оставшихся бит
  jmp LOOP_1 

 EXIT_LOOP_1:
ret

findDevice: ;Подпрограмма поиска устройства
 cli
 push TEMP_REG
 call setLineToZero ;Устанавка нуля на шине
 ldi XL, low(H) 
 ldi XH,  high(H)
 call wait ;выжидание
 call releaseBus ; освобождение шины
 ldi XL, I
 call wait ;выжидание
 in TEMP_REG, PIND
 sbrs TEMP_REG, SENSOR_LINE   
 ldi PRESENCE_REG, (0 << SENSOR_LINE)
 ldi XL, low(J) 
 ldi XH,  high(J)
 call wait ;выжидание
 pop TEMP_REG
 sei
ret

writeBiteOne: ;подпрограмма записи лог. 0
 cli
 call setLineToZero ;Устанавка нуля на шине
 ldi XL, A
 call wait ;выжидание
 call releaseBus ;освобождение шины
 ldi XL, B
 call wait ;выжидание
 sei
ret

writeBiteZero: ;подпрограмма записи лог. 1
 cli
 call setLineToZero ;Устанавка нуля на шине
 ldi XL, C
 call wait ;выжидание
 call releaseBus ;освобождение шины
 ldi XL, D
 call wait ;выжидание
 sei
ret

readBite: ;подпрограмма чтения бита
 cli
 call setLineToZero ;Устанавка нуля на шине
 ldi XL, A
 call wait ;выжидание
 call releaseBus ;освобождение шины
 ldi XL, E
 call wait  ;выжидание
 in r25, PIND
 sbrc r25, SENSOR_LINE
 breq WRITE_ONE ;
 ldi SENSOR_DATA_REG, 0 ;запись нуля
 jmp WAITING
  
 WRITE_ONE:
  ldi SENSOR_DATA_REG, 1 ;запись единицы

 WAITING:
  ldi XL, F 
  call wait

 sei
ret

setLineToZero:
 sbi DDRD, SENSOR_LINE
 cbi PORTD,SENSOR_LINE
ret

releaseBus:
 cbi DDRD, SENSOR_LINE
 cbi PORTD,SENSOR_LINE
ret

wait:
 sbiw X, 1
 cpi XH, 0
 brne wait 
 cpi XL, 0 
 brne wait
ret


