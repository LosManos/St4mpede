# Readme for St4mpede

St4mpede is a T4 solution for creating a full data access layer.
It is still an alpha.

The idea is to point a config to a relational database. St4mpede then examines its schema and creates POCOs and the rest of the DAL for simple CRUD operations. It is in no way perfect for the big solution but it makes you land with your feet running.
Note: Right now it barely creates the POCOs. It is still an alpha y'know.

## Thanks to
http://www.luisrocha.net/2008/07/testing-text-template-transformation.html
https://github.com/jbubriski/ST4bby
https://github.com/netTiers/netTiers
http://www.codesmithtools.com/freeware

## TODO:
ISettings should be internal, shouldn't it?
I tried to but the compiler complained and said it couldn't implement it because it wasn't public.
We might not need ISettings at all.
Move this Todo to Github.