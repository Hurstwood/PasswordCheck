# PasswordCheck
A C# program to check passwords against haveibeenpwned.com archive.  
Inspired by Mike Pound's video on Computerphile - https://www.youtube.com/watch?v=hhUb5iknVJs

The video shows a Python implementation.  I created this to allow family, friends, and anyone who doesn't know how to program the ability to check their password without submitting their password to a website. A compiled executable that you can download and run has been uploaded - PasswordCheck.exe

## Summary

Type your password into the program, click the check button and it will check with haveibeenpwned's archive to see if your password has been reported as collected by a bad person in a website breach.

## How to use / How it works

You can choose whether your inputted password is masked or in clear text.

As you input your password, the SHA1 hash of your password is immediately generated.

Press the enter/return key whilst focused on the input box or click the check button to initiate the check.

Only the first 5 characters of the 35 character SHA1 hash is sent to Haveibeenpwned.

Haveibeenpwned returns a list of all hashes that start with the same 5 characters.

The program then checks if your password is present in the list by checking the remaining 30 characters of your password hash with each hash returned.

If your password is present, it extracts the number of times it has been found and displays the results.

If your password is not present, it displays a simple message informing you of this.


Haveibeenpwned only allows one request every 1.5 seconds.  If you attempt to initiate a check within 1.5s of the last check, the program will wait until the period expires.


