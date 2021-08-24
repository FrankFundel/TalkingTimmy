import csv
import numpy as np
import os

face_fps = 24

def prepareBlend(path):
  filename, _ = os.path.splitext(path)
  header = []
  frames = []
  with open(path) as csvfile:
    reader = csv.reader(csvfile)
    header = next(reader)[2:] # get header
    curr_sec = 0
    block = []
    for row in reader:
      sec = row[0].split(":")[2]
      if curr_sec == 0: # group frames into 1 second blocks with varying sizes
        curr_sec = sec
      
      if curr_sec != sec:
        idx = np.round(np.linspace(0, len(block)-1, face_fps)).astype(int) # lower fps to 24 e.g. take 24 evenly spaced items out of block
        for i in idx:
          frames.append(block[i])
        block = []
        curr_sec = sec

      data = row[2:] # remove first two columns (timecode and num blendshapes)
      data = [float(string) for string in data] # convert string to float
      block.append(data)

  #write again
  with open(filename + "-prep.csv", 'w', newline='') as csvfile:
    writer = csv.writer(csvfile)
    writer.writerow(header)
    for row in frames:
      writer.writerow(row)

# With all folders
path = 'D:/Speech2Face/takes_2906/'
for dir in os.listdir(path):
  for file in os.listdir(path + dir):
    filepath = path + dir + "/" + file
    if file.endswith("csv") and "-prep" not in file:
      prepareBlend(filepath)