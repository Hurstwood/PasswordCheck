# PasswordCheck
A C# program to check passwords against haveibeenpwned.com archive.  
Inspired by Mike Pound's video on Computerphile - https://www.youtube.com/watch?v=hhUb5iknVJs

The video shows a Python implementation.  I created this to allow family, friends, and anyone who doesn't know how to program the ability to check their password without submitting their password to a website.

Type your password into the program, click the check button and it will check with haveibeenpwned's archive to see if your password has been reported as collected by a bad person in a website breach.

You can choose whether your password is masked or in clear text as you type it in.

As you input your password, the SHA1 hash of your password is immediately generated.  The clear text version of password is never stored in a variable.

Press the enter/return key whilst focused on the input box or click the check button to check haveibeenpwned.

Only the first 5 characters of the 35 SHA1 hash is sent to haveibeenpwned.

Haveibeenpwned returns a list of all hashes that start with the same 5 characters.

The program then checks if your password is in haveibeenpwned's archive by checking the remaining 30 characters of the hash against the list.

If your password is present, it extracts the number of times it has been found and displays the results.

If your password is not present, it displays this.


