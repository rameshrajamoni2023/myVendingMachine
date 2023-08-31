GoLogic Coding challenge
------------------------

Virtual Vending Machine
-----------------------

A) Description of the problem and solution:

1.Create a virtual vendening machine.
Solution: The vending machine functionality is implemented as APIs

2. The vending machine contains a list of products, with a corresponding amount and quantity available.
Solution: SeedData.sql file is added in the solution to upload products initially.
	  Also APIs AddProduct (add a single product) and UploadProduct( add products from csv file) 
           available to add products with amount and quantity.
          

3. Users put in (virtual) money and purchase an item.
Solution: APIs implemented to load money, and purchase an item(Product). 

4. After they have purchased an item, they can use the remaining money to purchase another item
Solution: API implemented to continue purchase till money is exhausted or collected from machine. 

5. or have the change returned to them.
Solution: API implemented to return money (remaining balance/credit) to user. Post this user need to add/load money back again
          before proceeding with the purchases.

6. Once they are done, they should see a list of the items they have purchased.
Solution: API implemented to see the list of items purchased as a receipt format.

7. Some basic business rules:

	a. Users cannot purchase a product if there is no quantity remaining
	Solution : A warning message "Item is out of stock" is displayed.
	b. Users can only purchase a product if they have put in funds equal to or greater than the cost of the product
	Solution : If funds are less than product price, user cannot purchase it , a warning message "Insufficient Funds" is displayed.
	c. Users should receive the correct change back after the transaction
	Solution : User are returned the remaining amount after the purchase. 
	   Note  : Currency denomination logic not implemented. User can enter any amount. 
	d. The product quantity should be reduced by the amount of quantity purchased of an item
	Solution : Based on the items purchased, the quantity is reduced.

-----------------------------------------------------------------------------------------------------------------------------------------
B) Whether the solution focuses on back-end, front-end or if it's full stack : 

   The vending machine functionality is implemented as APIs, to enable any type of UI to consume.
   A UI application (not fully implemented) developed using WPF is available for testing, to have a idea of User Experience.
   A more light weight UI using React JS and HTML and CSS would be ideal - not implemented due to time constraint and it is not my main skill.

C) Reasoning behind your technical choices, including architectural.

   My initial thought was to implement a SPA (single page application) having the whole functionality within (inmemory DB,platform independent and responsive UI). 
   But as the intent of the exercise was to showcase the candidates skill area, I chose a distributed architecture. 
	  
   I have used a distributed architecture (different layers/classes) but implemented them in a single C# project for simplicity. 
   Moreover we can separate the Service and DbContext logic to separate projects (libraries),
   so that they can be invidually containerized in a cloud and highly scaled.

D) Trade-offs you might have made, anything you left out, or what you might do differently if you were to spend additional time on the project.
   
   DB Side - Again I have kept the data model simplistic to capture the required details. It can be evolved to a enterprise level with more details
             related to audit and user actions captured. This was the idea behind the transaction and transactiondetails table,
             however due to time and scope, I have limited them to store a single sesson data. We can enhance it later as per need.

  Unit Tests - some unit tests are still pending. However current ones should give some input on my approach to it.Used MSTest and Moq.


E) Link to other code you're particularly proud of:
   
   I randomly googled the internet to find what kind of solutions have been made/proposed. One solution using code.org and another suggestion 
   to implement the same using a state transition design pattern were impressive.    

F) Link to your resume or public profile.
   Will share it in a email. 

G) Link to to the hosted application where applicable.  
   Not hosted anywhere as of now, will try to host in Azure cloud, using Azure Webapps.
  
-----------------------------------------------------------------------------------------------------------------------------------------------

The aspects of your code we will assess include:

H) Architecture: how clean is the separation between the front-end and the back-end?

   Have used different classes - API Layer -> Service Layer -> DB Layer -> DB .
   In Development mode - swagger UI is invoked to list the APIs and user has option to try them.

I) Clarity: does the README clearly and concisely explains the problem and solution? Are technical tradeoffs explained?
    
   I have tried my best to explain the details requested here. 
   I have used the asyn tasks for the API , however for this requirement (small data and single user per machine at a time) 
   performance aspect is not critical. This solution can be slightly modified (including DataBase logic) to support
   enterprise level multi-tenant requirements in future.

J) Correctness: does the application do what was asked? If there is anything missing, does the README explain why it is missing?

   I have covered all the requirements asked and exposed them as APIs. 
   APIs naming convention can be improved. Also the messages format are not consistent, need to be refactored like convert hardcoded messages 
   to consts or resource files to support globalization.

K) Code quality: is the code simple, easy to understand, and maintainable? Are there any code smells or other red flags? 
   Does object-oriented code follows principles such as the single responsibility principle? 
   Is the coding style consistent with the language's guidelines? Is it consistent throughout the codebase?

   Have followed the standard coding conventions. Naming conventions may be missed, need a review.
   Code is structured and naming conventions which user can easily understand is used.

L) Security: are there any obvious vulnerability?

   During testing , didn't find any. However lately I added a nice to have API to enable user to bulk upload 
   products from csv file. Security vulnerabilty check using tools/VS extentions not yet used, due to local laptop constraints. 

M) Testing: how thorough are the automated tests? Will they be difficult to change if the requirements 
of the application were to change? Are there some unit and some integration tests?

   Few test cases have been left out due to time limitation. MSTest and Moq have been used.

N) UX: is the web interface understandable and pleasing to use? Is the API intuitive?
   I have not focused on UI. UX is simple and clean , however it is not ready for business users, but only for tech team to test the APIs.
   It need to be refactored and cleaned.
   APIs return values mostly as string with addiional info so that UI need not worry to format the response - it can be changed as per need.
   Service layer uses interface to enable different implementatons. 

O) Technical choices: do choices of libraries, databases, architecture etc. seem appropriate for the chosen application?
   I have used as per industry standard and trending approaches.

P) Bonus point (those items are optional):
   Scalability: will technical choices scale well? If not, is there a discussion of those choices in the README?
   Production-readiness: does the code include monitoring? logging? proper error handling?

   The DB model supports upscaling to support transactions from multiple vending machines.
   The logging and error handling has been done at the API layer which is exposed. 
   Log is written into the console now which can be extended to a file or DB.
   The log entries can be used for monitoring using some observabiity tools like Grafana or loggly or splunk.
   Code can be easily extended to capture metrics for real time monitoring.

 NEXT STEPS

  API to update the price of a product.
  API to remove a product from the machine.
 
  Implement authentication and authorization for user access to API endpoints - using JWT (JSON Web Tokens), OAuth (MFA).
  Add Inventory Management - to keep track of products and place restocking order
  Add Payment Procesing - payment gateway.
  
  Include input validation, throttling, and handle vulnerabilities like SQL injection and CSRF.

--------------------------------------------------------------------------------------------------------------
NOTE:

I can provide other technical documents like UML (class diagrams) and DRD (data relationship diagrams)
on request. In Agile world such documents are rarely prepared unless complexity is high.

I can also share user stories if required.

We can have additional APIs if user action need to be captured.
   
-------------------------------------------------------------------------------------------------------------

  

