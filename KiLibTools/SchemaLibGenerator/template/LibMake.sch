EESchema Schematic File Version 2  date 2013/04/11 11:13:55
LIBS:Capacitor
LIBS:ConFe
LIBS:ConMa
LIBS:MOSFET
LIBS:PowerSupply
LIBS:Resistor
LIBS:Transistor
LIBS:PhotoDevice
LIBS:74xx
LIBS:device
LIBS:LibMake-cache
EELAYER 27 0
EELAYER END
$Descr A4 11693 8268
encoding utf-8
Sheet 1 1
Title ""
Date "11 apr 2013"
Rev ""
Comp ""
Comment1 ""
Comment2 ""
Comment3 ""
Comment4 ""
$EndDescr
$Comp
L D D3
U 1 1 5151F295
P 4350 4550
F 0 "D3" H 4300 4620 40  0000 L CNN
F 1 "D" H 4350 4470 40  0000 C CNN
F 2 "~" H 4350 4550 60  0000 C CNN
F 3 "~" H 4350 4550 60  0000 C CNN
	1    4350 4550
	0    -1   -1   0   
$EndComp
$Comp
L DSCH D2
U 1 1 5151FC8B
P 4350 4300
F 0 "D2" H 4300 4370 40  0000 L CNN
F 1 "DSCH" H 4350 4220 40  0000 C CNN
F 2 "~" H 4350 4300 60  0000 C CNN
F 3 "~" H 4350 4300 60  0000 C CNN
	1    4350 4300
	0    -1   -1   0   
$EndComp
$Comp
L DZ D1
U 1 1 5151FD00
P 4350 4050
F 0 "D1" H 4300 4120 40  0000 L CNN
F 1 "DZ" H 4350 3970 40  0000 C CNN
F 2 "~" H 4350 4050 60  0000 C CNN
F 3 "~" H 4350 4050 60  0000 C CNN
	1    4350 4050
	0    -1   -1   0   
$EndComp
$Comp
L L-TYPE1 L3
U 1 1 5155168E
P 4700 4600
F 0 "L3" H 4575 4660 40  0000 L CNN
F 1 "L-TYPE1" H 4700 4550 40  0000 C CNN
F 2 "~" V 4630 4600 30  0000 C CNN
F 3 "~" H 4700 4600 30  0000 C CNN
F 4 "4.7m" H 4725 4660 35  0000 L CNN "Inductance"
	1    4700 4600
	1    0    0    -1  
$EndComp
$Comp
L L-TYPE2 L2
U 1 1 51553EBA
P 4700 4400
F 0 "L2" H 4575 4480 40  0000 L CNN
F 1 "L-TYPE2" H 4700 4350 40  0000 C CNN
F 2 "~" V 4630 4400 30  0000 C CNN
F 3 "~" H 4700 4400 30  0000 C CNN
F 4 "4.7m" H 4725 4480 35  0000 L CNN "Inductance"
	1    4700 4400
	1    0    0    -1  
$EndComp
$Comp
L L-TYPE3 L1
U 1 1 51553ED3
P 4700 4200
F 0 "L1" H 4575 4280 40  0000 L CNN
F 1 "L-TYPE3" H 4700 4150 40  0000 C CNN
F 2 "~" V 4630 4200 30  0000 C CNN
F 3 "~" H 4700 4200 30  0000 C CNN
F 4 "4.7m" H 4725 4280 35  0000 L CNN "Inductance"
	1    4700 4200
	1    0    0    -1  
$EndComp
$Comp
L TR-NPN Q1
U 1 1 5155C05E
P 5450 4100
F 0 "Q1" H 5550 4200 40  0000 L CNN
F 1 "TR-NPN" H 5550 4000 40  0000 L CNN
F 2 "~" H 5400 4100 60  0000 C CNN
F 3 "~" H 5400 4100 60  0000 C CNN
	1    5450 4100
	1    0    0    -1  
$EndComp
$Comp
L TR-PNP Q4
U 1 1 5155C06D
P 5900 4100
F 0 "Q4" H 6000 4200 40  0000 L CNN
F 1 "TR-PNP" H 6000 4000 40  0000 L CNN
F 2 "~" H 5850 4100 60  0000 C CNN
F 3 "~" H 5850 4100 60  0000 C CNN
	1    5900 4100
	1    0    0    -1  
$EndComp
$Comp
L JFET-P Q2
U 1 1 5155C07C
P 5450 4450
F 0 "Q2" H 5550 4550 40  0000 L CNN
F 1 "JFET-P" H 5550 4350 40  0000 L CNN
F 2 "~" H 5240 4552 29  0000 C CNN
F 3 "~" H 5400 4450 60  0000 C CNN
	1    5450 4450
	1    0    0    -1  
$EndComp
$Comp
L JFET-N Q5
U 1 1 5155C09A
P 5900 4450
F 0 "Q5" H 6000 4550 40  0000 L CNN
F 1 "JFET-N" H 6000 4350 40  0000 L CNN
F 2 "~" H 5690 4552 29  0000 C CNN
F 3 "~" H 5850 4450 60  0000 C CNN
	1    5900 4450
	1    0    0    -1  
$EndComp
$Comp
L FET-N Q3
U 1 1 5155C0A9
P 5450 4800
F 0 "Q3" H 5550 4900 40  0000 L CNN
F 1 "FET-N" H 5550 4700 40  0000 L CNN
F 2 "~" H 5400 4800 60  0000 C CNN
F 3 "~" H 5400 4800 60  0000 C CNN
	1    5450 4800
	1    0    0    -1  
$EndComp
$Comp
L FET-P Q6
U 1 1 5155C0B8
P 5900 4800
F 0 "Q6" H 6000 4900 40  0000 L CNN
F 1 "FET-P" H 6000 4700 40  0000 L CNN
F 2 "~" H 5850 4800 60  0000 C CNN
F 3 "~" H 5850 4800 60  0000 C CNN
	1    5900 4800
	1    0    0    -1  
$EndComp
$Comp
L SW-SPST SW5
U 1 1 5155C841
P 5300 3750
F 0 "SW5" H 5200 3830 40  0000 L CNN
F 1 "SW-SPST" H 5300 3680 40  0000 C CNN
F 2 "~" H 5300 3750 60  0000 C CNN
F 3 "~" H 5300 3750 60  0000 C CNN
	1    5300 3750
	1    0    0    -1  
$EndComp
$Comp
L SW-SPDT SW6
U 1 1 5155CAD3
P 5700 3700
F 0 "SW6" H 5600 3800 40  0000 L CNN
F 1 "SW-SPDT" H 5700 3575 40  0000 C CNN
F 2 "~" H 5700 3700 60  0000 C CNN
F 3 "~" H 5700 3700 60  0000 C CNN
	1    5700 3700
	1    0    0    -1  
$EndComp
$Comp
L SW-DPST SW4
U 1 1 5155CEB6
P 5300 3450
F 0 "SW4" H 5200 3580 40  0000 L CNN
F 1 "SW-DPST" H 5300 3325 40  0000 C CNN
F 2 "~" H 5300 3450 60  0000 C CNN
F 3 "~" H 5300 3450 60  0000 C CNN
	1    5300 3450
	1    0    0    -1  
$EndComp
$Comp
L SW-DPST-TACT SW2
U 1 1 5155CFD0
P 4850 3450
F 0 "SW2" H 4750 3600 40  0000 L CNN
F 1 "SW-DPST-TACT" H 4850 3330 40  0000 C CNN
F 2 "~" H 4850 3450 60  0000 C CNN
F 3 "~" H 4850 3450 60  0000 C CNN
	1    4850 3450
	1    0    0    -1  
$EndComp
$Comp
L SW-SPST-TACT SW3
U 1 1 5155D018
P 4850 3750
F 0 "SW3" H 4750 3850 40  0000 L CNN
F 1 "SW-SPST-TACT" H 4850 3680 40  0000 C CNN
F 2 "~" H 4850 3750 60  0000 C CNN
F 3 "~" H 4850 3750 60  0000 C CNN
	1    4850 3750
	1    0    0    -1  
$EndComp
$Comp
L SW-DPST-TACT-2 SW1
U 1 1 5155D19F
P 4850 3100
F 0 "SW1" H 4750 3250 40  0000 L CNN
F 1 "SW-DPST-TACT-2" H 4850 2980 40  0000 C CNN
F 2 "~" H 4850 3100 60  0000 C CNN
F 3 "~" H 4850 3100 60  0000 C CNN
	1    4850 3100
	1    0    0    -1  
$EndComp
$Comp
L +11.1V #PWR?
U 1 1 5157A02F
P 6300 3200
F 0 "#PWR?" H 6300 3380 40  0001 C CNN
F 1 "+11.1V" H 6300 3325 40  0000 C CNN
F 2 "~" H 6300 3200 60  0000 C CNN
F 3 "~" H 6300 3200 60  0000 C CNN
	1    6300 3200
	1    0    0    -1  
$EndComp
$Comp
L -10V #PWR?
U 1 1 5157A064
P 6300 3300
F 0 "#PWR?" H 6300 3105 40  0001 C CNN
F 1 "-10V" H 6300 3160 40  0000 C CNN
F 2 "~" H 6300 3300 60  0000 C CNN
F 3 "~" H 6300 3300 60  0000 C CNN
	1    6300 3300
	1    0    0    -1  
$EndComp
$Comp
L GND2 #PWR?
U 1 1 5157A7C4
P 6500 3300
F 0 "#PWR?" H 6500 3105 40  0001 C CNN
F 1 "GND2" H 6500 3160 40  0000 C CNN
F 2 "~" H 6500 3300 60  0000 C CNN
F 3 "~" H 6500 3300 60  0000 C CNN
	1    6500 3300
	1    0    0    -1  
$EndComp
$Comp
L LED(PHI5) LED?
U 1 1 51661A48
P 4050 4550
F 0 "LED?" H 3950 4670 40  0000 L CNN
F 1 "LED(PHI5)" H 4050 4470 40  0000 C CNN
F 2 "~" H 4050 4550 60  0000 C CNN
F 3 "~" H 4050 4550 60  0000 C CNN
	1    4050 4550
	0    -1   -1   0   
$EndComp
$Comp
L C(CERA) C?
U 1 1 51661D91
P 4950 4850
F 0 "C?" H 5000 4920 45  0000 L CNN
F 1 "C(CERA)" H 5000 4775 45  0000 L CNN
F 2 "~" H 4988 4700 30  0000 C CNN
F 3 "~" H 4950 4850 60  0000 C CNN
	1    4950 4850
	1    0    0    -1  
$EndComp
$Comp
L C(POLE) C?
U 1 1 5166204C
P 4500 4850
F 0 "C?" H 4550 4920 45  0000 L CNN
F 1 "C(POLE)" H 4550 4775 45  0000 L CNN
F 2 "~" H 4538 4700 30  0000 C CNN
F 3 "~" H 4500 4850 60  0000 C CNN
	1    4500 4850
	1    0    0    -1  
$EndComp
$Comp
L C(NPOLE) C?
U 1 1 5166205B
P 4200 5000
F 0 "C?" H 4250 5070 45  0000 L CNN
F 1 "C(NPOLE)" H 4250 4925 45  0000 L CNN
F 2 "~" H 4238 4850 30  0000 C CNN
F 3 "~" H 4200 5000 60  0000 C CNN
	1    4200 5000
	1    0    0    -1  
$EndComp
$Comp
L R(1005M) R?
U 1 1 5166230D
P 5100 4350
F 0 "R?" H 4975 4410 45  0000 L CNN
F 1 "R(1005M)" H 5100 4280 45  0000 C CNN
F 2 "~" V 5030 4350 30  0000 C CNN
F 3 "~" H 5100 4350 30  0000 C CNN
	1    5100 4350
	1    0    0    -1  
$EndComp
$Comp
L CON-FEMALE-DOUBLE-10 CN?
U 1 1 51662D14
P 6950 4000
F 0 "CN?" H 6800 4280 40  0000 L CNN
F 1 "CON-FEMALE-DOUBLE-10" H 6950 3710 40  0000 C CNN
F 2 "~" H 6900 4225 60  0000 C CNN
F 3 "~" H 6900 4225 60  0000 C CNN
	1    6950 4000
	1    0    0    -1  
$EndComp
$Comp
L CON-MALE-DOUBLE-10 CN?
U 1 1 51662D23
P 6950 4700
F 0 "CN?" H 6800 4980 40  0000 L CNN
F 1 "CON-MALE-DOUBLE-10" H 6950 4410 40  0000 C CNN
F 2 "~" H 6900 4925 60  0000 C CNN
F 3 "~" H 6900 4925 60  0000 C CNN
	1    6950 4700
	1    0    0    -1  
$EndComp
$EndSCHEMATC