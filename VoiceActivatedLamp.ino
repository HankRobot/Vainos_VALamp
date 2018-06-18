#include <SoftwareSerial.h>

#include <Adafruit_NeoPixel.h>
#define color 200

Adafruit_NeoPixel strip = Adafruit_NeoPixel(1, 5, NEO_GRB + NEO_KHZ800);

uint32_t purple = strip.Color(150, 0, color);
uint32_t red = strip.Color(color, 0, 0);
uint32_t green = strip.Color(0, color, 0);
uint32_t yellow = strip.Color(color, color, 0);
uint32_t blue = strip.Color(0, 0, color);
uint32_t cyan = strip.Color(0, color, color);
uint32_t white = strip.Color(color, color, color);
uint32_t blank = strip.Color(0, 0, 0);
uint32_t rainbowBit_color[7] = {red, green, blue, purple, yellow, cyan, white};

boolean CommandDone = false;
char colorVal = 'x';
SoftwareSerial mySerial(2, 3);

void setup() {
  // put your setup code here, to run once:
  mySerial.begin(9600);
  Serial.begin(9600);
  pinMode(5,OUTPUT);
  pinMode(A0, OUTPUT);
  pinMode(A1, OUTPUT);
  pinMode(A2, INPUT);
  digitalWrite(A0, HIGH);
  digitalWrite(A1, LOW);
  strip.begin();  
  delay(100); 
  strip.show();
}
int counter = 0;
int cR, cG, cB = 0;
void loop() {
    counter ++;
    while(Serial.available())
    {
      colorVal = Serial.read();
      CommandDone = true;
    }
    switch(colorVal){
      case 'r':
        strip.setPixelColor(0, red);
        break;
      case 'g':
        strip.setPixelColor(0, green);
        break;
      case 'b':
        strip.setPixelColor(0, blue);
        break;
      case 'w':
        strip.setPixelColor(0, white);
        break;
      case 'p':
        strip.setPixelColor(0, purple);
        break;
      case 'y':
        strip.setPixelColor(0, yellow);
        break;
      case 'e':
        strip.setPixelColor(0, blank);
        break;
      case 'd':
        if(counter%2 ==0)
        {
          cR = random(0, 200);
          cG = random(0, 200);
          cB = random(0, 200);
          strip.setPixelColor(0, strip.Color(cR, cG, cB));
          delay(200);
        }
        else if(counter < 5000)
        {
          strip.setPixelColor(0, blank);
          delay(100);
        }
        else if(counter < 3000)
        {
          cR = random(0, 200);
          cG = random(0, 200);
          cB = random(0, 200);
          strip.setPixelColor(0, strip.Color(cR, cG, cB));
          delay(500);
        }
        else
        {
          counter = 0;
        }
        break;
      case '!':
        if(counter<1)
        {
          cR = 0;
          cG = 200;
          cB = 0;
            
        }
        else if(counter < 200)
          {
            cR = 0;
            cG = 200;
            cB += 1;
            delay(5);
          }
        else if(counter < 300)
          {
            cR = 0;
            cG -= 1;
            cB = 200;
          }
         else if(counter < 400)
         {
            cR = 0;
            cG += 1;
            cB -= 2;
         }
         else
         {
            cR = 0;
            cG = 200;
            cB = 0;
            counter = 0;
         }
         strip.setPixelColor(0, strip.Color(cR, cG, cB));
         delay(10);
         break;
          
      case 'k':
        if(counter < 500)
        {
          strip.setPixelColor(0, yellow);
        }
        else if(counter < 1000)
        {
          strip.setPixelColor(0, red);
        }
        else
        {
          counter = 0;
        }
        break;
      case 'x':
        if(counter<1)
        {
          cR = 200;
          cG = 0;
          cB = 0;
            
        }
          else if(counter < 200)
          {
            cR = 200;
            cG += 1;
            cB = 0;
          }
          else if (counter <400)
          {
            cR -= 1;
            cG = 200;
            cB = 0;
          }
          else if (counter <600)
          {
            cR = 0;
            cG = 200;
            cB += 1;
          }
          else if (counter <800)
          {
            cR = 0;
            cG -= 1;
            cB = 200;
          }
          else if (counter <1000)
          {
            cR += 1;
            cG = 0;
            cB = 200;
          }
          else if (counter <1200)
          {
            cR = 200;
            cG = 0;
            cB -= 1;
          }
          else
          {
            cR = 200;
            cG = 0;
            cB = 0;
            counter = 0;
          }
          strip.setPixelColor(0, strip.Color(cR, cG, cB));
          delay(10);
          break;
      default:
        break;
    }
    strip.show();
  //strip.setPixelColor(0, white);
  //strip.show();
  //delay(2000); 
  //strip.setPixelColor(0, red);
  //strip.show();
}
