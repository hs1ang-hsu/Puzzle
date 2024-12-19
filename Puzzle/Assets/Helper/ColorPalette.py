# source: https://www.rapidtables.com/web/color/RGB_Color.html
colors = {
    "maroon": (128,0,0),
    "orange red": (255,69,0),
    "light coral": (240,128,128),
    "dark orange": (255,140,0),
    "gold": (255,215,0),
    "golden rod": (218,165,32),
    "spring green": (0,255,127),
    "yellow green": (154,205,50),
    "forest green": (34,139,34),
    "torquoise": (64,224,208),
    "teal": (0,128,128),
    "corn flower blue": (100,149,237),
    "deep sky blue": (0,191,255),
    "navy": (0,0,128),
    "indigo": (75,0,130),
    "dark violet": (148,0,211),
    "orchid": (218,112,214),
    "peru": (205,133,63),
    "ivory": (255,255,240)
}

for it in colors.items():
    color = it[1]
    name = it[0].title().replace(' ', '')
    print(f"\tpublic static Color {name} = new Color({round(color[0]/255,3)}f, {round(color[1]/255,3)}f, {round(color[2]/255,3)}f);")