import cv2
import os

files = os.listdir(r"C:\Drive\Unity\Puzzle\Puzzle\Assets\Resources\Puzzles")
for f in files:
    if 'png' not in f or 'meta' in f:
        continue
    img = cv2.imread(f, cv2.IMREAD_UNCHANGED)
    name = f.split('.')[0]
    for r in range(4):
        new_name = name + f'_r{r}'
        if r == 0:
            os.rename(f, new_name + '.png')
        else:
            cv2.imwrite(new_name + '.png', img)
        flip_img = cv2.flip(img, 1)
        cv2.imwrite(new_name + 'f.png', flip_img)
        img = cv2.rotate(img, cv2.ROTATE_90_CLOCKWISE)