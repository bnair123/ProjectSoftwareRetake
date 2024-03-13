/*
 * New implementation:
 * - Limit function has been set up
 * To introduce limit write into console limit:[number] replace [num] with yur actual number
 * Activates limit LED and buzzer (I think, tinker cad doesn't seem to be making any noise so idk) when limit reached
 * still need to make it based on reset button long press
 * Current Issues:
 * - Hundreds display is broken
 * - Long press not implemented
 * - Limit function
 * - Add Doxygen style comments
 * - (additional features such as display GUI and API connection)
 */
 // Define shift register pins
const int latchPin = A1;  // RCLK
const int clockPin = A0;  // SRCLK
const int dataPin = A2;   // Serial

// Define button pins 
const int button1Pin = 13;
const int button2Pin = 12;
const int button3Pin = 11;

// Define LED and buzzer pins
const int ledPin1 = A3; // LED for general indication
const int ledPin2 = A4; // LED for limit indication
const int buzzerPin = A6; // Buzzer for limit indication

// Define CD4511 pins (tens display)
const int d0Pin = 7;
const int d1Pin = 8;
const int d2Pin = 9; 
const int d3Pin = 10;

// Define pins for hundreds display
const int hundreds[] = {6, 5, 4, 3, 2, 1, 0};

// Counter value
int count = 0;

// Counter limit value
int limit = 999; // Default limit

void setup() {
  Serial.begin(9600);
  
  // Set shift register pins as outputs
  pinMode(latchPin, OUTPUT);
  pinMode(clockPin, OUTPUT); 
  pinMode(dataPin, OUTPUT);
  
  // Set CD4511 pins as outputs
  pinMode(d0Pin, OUTPUT);
  pinMode(d1Pin, OUTPUT);
  pinMode(d2Pin, OUTPUT);
  pinMode(d3Pin, OUTPUT);
  
  // Set LED and buzzer pins as outputs
  pinMode(ledPin1, OUTPUT);
  pinMode(ledPin2, OUTPUT);
  pinMode(buzzerPin, OUTPUT);
  
  // Set hundreds display pins as outputs
  for(int i = 0; i < 7; i++) {
    pinMode(hundreds[i], OUTPUT);
  }
  
  // Set button pins as inputs with pullup resistors
  pinMode(button1Pin, INPUT);
	pinMode(button2Pin, INPUT);
	pinMode(button3Pin, INPUT);

}

void loop() {
  if (Serial.available() > 0) {
        String command = Serial.readStringUntil('\n');
        if (command.startsWith("limit:")) {
            int newLimit = command.substring(6).toInt();
            if (newLimit >= 0 && newLimit <= 999) {
                limit = newLimit;
                Serial.print("New limit set: ");
                Serial.println(limit);
            } else {
                Serial.println("Invalid limit");
            }
        } else {
            // Process other serial commands or values
            int incomingNumber = command.toInt();
            if (incomingNumber >= 0 && incomingNumber <= 999) {
                count = incomingNumber;
                Serial.print("New count set: ");
                Serial.println(count);
            }
        }
        while (Serial.available() > 0) Serial.read(); // Clear the serial buffer.
    }

  // Update display
  updateDisplay(count);
  // Check button 1
  if(digitalRead(button1Pin) == HIGH) { // Change to HIGH for active high buttons
    if (count < limit) {
      count += 1;  // Increment by 1
      digitalWrite(ledPin1, HIGH);  // Turn on action LED
      delay(100);  // Short delay for visibility
      digitalWrite(ledPin1, LOW);
    } else {
      // Limit reached, activate buzzer and LED
      digitalWrite(buzzerPin, HIGH);
      digitalWrite(ledPin2, HIGH);
      delay(500);  // Keep on for visibility
      digitalWrite(buzzerPin, LOW);
      digitalWrite(ledPin2, LOW);
    }
    while(digitalRead(button1Pin) == HIGH); // Wait for button release
    Serial.println("Button 1 pressed, count changed.");
  }
  
  // Check button 2
  if(digitalRead(button2Pin) == HIGH) { // Change to HIGH
    if (count > 0) {
      count--;  // Decrement by 1
      digitalWrite(ledPin1, HIGH);  // Turn on action LED
      delay(100);  // Short delay for visibility
      digitalWrite(ledPin1, LOW);
    } else {
      // Limit reached, activate buzzer and LED
      digitalWrite(buzzerPin, HIGH);
      digitalWrite(ledPin2, HIGH);
      delay(500);  // Keep on for visibility
      digitalWrite(buzzerPin, LOW);
      digitalWrite(ledPin2, LOW);
    }
    while(digitalRead(button1Pin) == HIGH); // Wait for button release
    Serial.println("Button 2 pressed, count changed.");
  }
  
  // Check button 3 (reset)
  if(digitalRead(button3Pin) == HIGH) { // Change to HIGH
    count = 0;
    Serial.println("Button 3 pressed, count reset to 0."); 
    while(digitalRead(button3Pin) == HIGH); // Wait for button release
  }

  // Update display
  updateDisplay(count);
  Serial.print("Current count: ");
  Serial.println(count);
  
  // Check if limit is reached
    if (count >= limit) {
        digitalWrite(ledPin2, HIGH); // Turn on limit LED
        digitalWrite(buzzerPin, HIGH); // Activate buzzer
        delay(100); 
        digitalWrite(ledPin2, LOW); // Turn off limit LED
        digitalWrite(buzzerPin, LOW); // Deactivate buzzer
    } else {
        digitalWrite(ledPin2, LOW); // Ensure limit LED is off if above limit
        digitalWrite(buzzerPin, LOW); // Ensure buzzer is off if above limit
    }
}

void updateDisplay(int value) {
  // Convert value to digits
  byte digit1 = value % 10;         
  byte digit2 = (value / 10) % 10;  
  byte digit3 = (value / 100) % 10;
  
  // Set CD4511 inputs (tens display)
  digitalWrite(d0Pin, digit2 & 1);
  digitalWrite(d1Pin, (digit2 >> 1) & 1);
  digitalWrite(d2Pin, (digit2 >> 2) & 1);
  digitalWrite(d3Pin, (digit2 >> 3) & 1);
  
  // Shift out ones digit
  digitalWrite(latchPin, LOW);
  shiftOut(dataPin, clockPin, MSBFIRST, getSevenSegmentCode(digit1)); 
  digitalWrite(latchPin, HIGH);
  
  // Set hundreds display for common cathode
  byte code = getSevenSegmentCode(digit3); 
  digitalWrite(hundreds[0], (code >> 0) & 1); // A
  digitalWrite(hundreds[1], (code >> 1) & 1); // B
  digitalWrite(hundreds[2], (code >> 2) & 1); // C
  digitalWrite(hundreds[3], (code >> 3) & 1); // D
  digitalWrite(hundreds[4], (code >> 4) & 1); // E
  digitalWrite(hundreds[5], (code >> 5) & 1); // F
  digitalWrite(hundreds[6], (code >> 6) & 1); // G
}


byte getSevenSegmentCode(int num) {
  switch(num) {
    case 0: return 0b00111111; // ABCDEF 
    case 1: return 0b00000110; // BC 
    case 2: return 0b01011011; // ABGED 
    case 3: return 0b01001111; // ABGCD 
    case 4: return 0b01100110; // FGBC 
    case 5: return 0b01101101; // AFGCD 
    case 6: return 0b01111101; // AFGCED 
    case 7: return 0b00000111; // ABC 
    case 8: return 0b01111111; // ABCDEFG 
    case 9: return 0b01101111; // ABFGCD
    default: return 0b00000000; // No segments lit
  }
}

