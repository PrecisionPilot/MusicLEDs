int value1 = 0;
void setup() {
  pinMode(3, OUTPUT);
  digitalWrite(3, HIGH);
  
  Serial.begin(1000000);
  Serial.setTimeout(8);
  
}
void loop() {
  
  while(Serial.available() == 0){}
  
  analogWrite(3, Serial.readString().substring(0,3).toInt());
  
  Serial.flush();
}
