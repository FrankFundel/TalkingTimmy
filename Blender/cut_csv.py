import os
import csv
import sys

filepath = sys.argv[1]
rem = int(sys.argv[2])

#read
keyframes = []
with open(filepath, mode='r') as csvfile:
  reader = csv.reader(csvfile)

  for row in reader:
    keyframes.append(row)

#write
with open(filepath, mode='w', newline='') as csvfile:
  writer = csv.writer(csvfile)

  writer.writerow(keyframes[0])
  for row in keyframes[rem+1:]:
    writer.writerow(row)
