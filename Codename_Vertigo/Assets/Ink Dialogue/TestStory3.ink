-> main

=== main ===
Here is a small test dialogue to stress test this system! #speaker:The Almighty #layout:right #portrait:None
Make a choice!
    + [Well, this is odd]
        ->chosen("Well, this is odd")
    + [This is not so bad]
        ->chosen("This is not so bad")
    + [This is very good actually]
        ->chosen("This is very good actually")


=== chosen(choice) ===
{choice}! #speaker:Rock Boy #layout:left #portrait:None
-> END