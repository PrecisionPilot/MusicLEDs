const int count = 15;
const int offset = 0;
void setup() {
  for(int i = 2; i < count + 2; i++)
  {
    if (i <= 13) { pinMode(i, OUTPUT); }
  }
  pinMode(A0, OUTPUT);
  pinMode(A1, OUTPUT);
  pinMode(A2, OUTPUT);
  pinMode(A3, OUTPUT);
  pinMode(A4, OUTPUT);
  pinMode(A5, OUTPUT);
  
  Serial.begin(1000000);
  Serial.setTimeout(8);
  
}
void loop() {
  String output = Serial.readString();
  
  int value1 = output.substring(0, 3).toInt();
  int value2 = output.substring(3, 6).toInt();
  int ledsOn = map(value1, 0, 255, 0, count);
  
  //clear
  for (int i = 2 + offset; i < count + 2 + offset; i++)
  {
    digitalWrite(i, LOW);
  }
  digitalWrite(A5, LOW);
  digitalWrite(A4, LOW);
  digitalWrite(A3, LOW);
  digitalWrite(A2, LOW);
  digitalWrite(A1, LOW);
  digitalWrite(A0, LOW);
  /*
  //Normal
  for (int i = 2 + offset; i < ledsOn + 2 + offset; i++)
  {
    digitalWrite(i, HIGH);
  }
  if (ledsOn >= 13 + offset) { digitalWrite(A5, HIGH); }
  if (ledsOn >= 14 + offset) { digitalWrite(A4, HIGH); }
  if (ledsOn >= 15 + offset) { digitalWrite(A3, HIGH); }
  if (ledsOn >= 16 + offset) { digitalWrite(A2, HIGH); }
  if (ledsOn >= 17 + offset) { digitalWrite(A1, HIGH); }
  if (ledsOn >= 18 + offset) { digitalWrite(A0, HIGH); }
  */
  //Reverse
  for (int i = count + 1; i > count - ledsOn + 1; i--)
  {
    digitalWrite(i, HIGH);
  }
  if (ledsOn >= 13 - count) { digitalWrite(A0, HIGH); }
  if (ledsOn >= 14 - count) { digitalWrite(A1, HIGH); }
  if (ledsOn >= 15 - count) { digitalWrite(A2, HIGH); }
  if (ledsOn >= 16 - count) { digitalWrite(A3, HIGH); }
  if (ledsOn >= 17 - count) { digitalWrite(A4, HIGH); }
  if (ledsOn >= 18 - count) { digitalWrite(A5, HIGH); }
  
  if (value2 > 0)
    analogWrite(3, value2);
  Serial.flush();
  while(Serial.available() == 0){}
}
