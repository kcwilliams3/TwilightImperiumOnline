TO CREATE ANOTHER LANGUAGE:

Step 1: Duplicate english.tilang and rename as the appropriate 
language ( ________.tilang ). The underlined portion there is 
what will show in the in-game options menu.
Then fill in the new file using the following formatting example:

<{>
english string 1,french string 1<;>
english string 2,french string 2<;>
english string 3,french string 3<;>
<}>


Step 2: For every text asset in Assets/Text/English  
(Assets/Processed/Text/English in the development build), write 
an equivalent file in the appropriate language. 
Note: You MUST use the data category names you provided in the 
.tilang file from step 1, as the file reader uses those names to
know what each piece of data represents.
Note that all text file names must be the same as the equivalent
english files (just in a different language directory).

Step 3: If you want the image assets that have text in them to
display text in the correct language, you will also need to
provide equivalent image files in the appropriate folder. 
(For example, any images in Resources/Systems/English would need
an equivalent in Resources/Systems/New Language .) If the file 
reader cannot find an image in the language-appropriate folder,
it will simply use the equivalent english image.
Note that system images must use the localized version of the
system's name (whatever appears as the system's name in your 
language's .tilang file). Systems without names follow the 
following format: Planet 1 NamePlanet 2 NamePlanet 3 Name