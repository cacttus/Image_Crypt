
# Image Dicer

Scramble an image into a _reversible_ montage.

![Preview](./preview.png)

Masking Feature
![Masking Feature](./preview-mask.jpg)

# Build

* Windows: Compile and run with Windows tools (visual studio).

* Linux: Compile this with Mono or MonoDevelop. Run mono 

# File Formats

Accepts Bmp, Jpg, Gif, Png formats. 
_With Jpg compression there may be visual artifacts when decrypting._

# Command Line Options

*Key, xsize, ysize, mask, and iterations all need to be the same to successfully decrypt an image.*

*key* = Set key to the value you want to unlock the image.

*xsize/ysize* = For encrypting an image set x = 1 y = 1 as the size parameter. For a cubism montage set this higher (1080 x 512)

*iterations* = This will go over the pixels multiple times for greater scrambling: 3= very scrambled image, 1= light scrambling.

*mask* = Randomly change the colors of the pixels.

# TODO

Add an image resizer to scale down the output

Add a way to encrypt images that takes just a key (e.g. generate xsize ysize mask and iteration from the input key)

Command line arguments.
