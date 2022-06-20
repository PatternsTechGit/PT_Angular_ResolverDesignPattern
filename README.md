# Angular Resolver Design Pattern

Developing a real-world application with multiple calls to the server can be full of bugs. These delays may cause a negative UX. Today, we are going to understand Route Resolvers in Angular. 

## What is a Resolver design pattern in Angular?

A Resolver is a class that **implements the Resolve interface** of Angular Router. A Resolver **acts like middleware**, which can be **executed before a component is loaded** and Resolver class will **fetch your data before the component is ready**. It has to be provided in the root module.  

---------------

## About this exercise

In this lab we will be working on the **Frontend Code Base** only. We will only call Accounts APIs from the previously designed labs.

### **Backend Code Base:**

We have developed Accounts APIs which are **GetAllAccounts** and **GetAllAccountsPaginated**. We will call the followings API to get all accounts which will return the result as in the image given below
http://localhost:5070/api/Accounts/GetAllAccounts

![](/BBBank_UI/src/assets/images/allAccounts.png)


There are 4 Projects in the solution. 

*	Entities : This project contains DB models like User where each User has one Account and each Account can have one or many Transactions. There is also a Response Model of LineGraphData that will be returned as API Response. 

*	Infrastructure: This project contains BBBankContext that service as fake DBContext that populates one User with its corresponding Account that has three Transactions dated of last three months with hardcoded data. 

* Services: This project contains AccountsService 

* BBBankAPI: This project contains AccountsController with 2 GET methods  **GetAllAccounts** and **GetAllAccountsPaginated** to call the AccountsService.

![](/BBBank_UI/src/assets/images/4.png)

For more details about this base project See: https://github.com/PatternsTechGit/PT_ServiceOrientedArchitecture

-----------

### **Frontend Code Base:**

Previously we scaffolded a new Angular application in which we have integrated Bootstrap navigation bar

![](/BBBank_UI/src/assets/images/1.png)

_____________

## In this exercise

* We will create client side models to receive data
* We will create accounts service to call the API
* We will be implementing resolver pattern to resolve the data for the route 
* We will populate the Html table using the response from the API

### **Step 1: Creating client side model**

We will create two interfaces for **Account** and **User** to receive data, like given below

***Account Interface***
```ts
import { User } from "./user";

export interface Account {

    accountTitle: string;
    user: User;
    currentBalance: number;
    accountStatus: number;
}
```
***User Interface***
```ts
export interface User {
    profilePicUrl: string;
}
```

----------------

### **Step 2: Set API Url Base in Environment Variable**


To set this up

* Copy the Base URL from our API
* Create a variable `apiUrlBase` in our environment script file
* Assign this Url to the variable as show below

```ts
export const environment = {
  apiUrlBase: 'http://localhost:5070/api/',
  redirectUri: 'http://localhost:4200', 
};


export default environment;
```
-------------------

 ### **Step 3: Create an accounts service**

 To create a account service we can follow these steps:
 * First import HttpClientModule in *module.ts* file

 ```ts
 import { HttpClientModule } from '@angular/common/http';

 imports: [
    HttpClientModule 
  ]
 ```

  * Now we will create service named `account` from terminal using this command

  ```bash
  ng generate service account
  ```
  * In this service we will first import *HttpClient*, *Account Model*, *Observable*, and *environment* file we just created we have created
  * Implement *getAllAccounts* method which will return an array of accounts in response of observable type.
  ```ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Account } from '../models/account';


@Injectable({
  providedIn: 'root',
})
export default class AccountsService {
  constructor(private httpClient: HttpClient) { }
}
  ```
  * On the Api we have a function called *getAllAccounts* that takes in the userId as parameter and can be accessed at location Accounts/getAllAccounts 

  Create a function called getAllAccounts in Accounts service to get all the accounts from the API. It will returns Observable of Array<Account> after hitting the api using httpClients Get verb. We have used the *apiUrlBase* from the environment file as the base URL 

  ```ts
    getAllAccounts(): Observable<Array<Account>> {
    return this.httpClient.get<Array<Account>>(`${environment.apiUrlBase}Accounts/GetAllAccounts`);
  }
  ```
  ### **Step 4: Call the API and store the data**


  * Create a new component named as *account.component* by running the following command in the terminal 
  ```bash
  ng generate component account
  ```
Import the following in your component
  ```ts
  // Importing LineGraphData model and TransactionService
import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Account } from '../models/account';
  
  ```
Inject the ActivatedRoute in the constructor of the service 
// ActivatedRoute from Angular has data that is associated with the route. Route params, Query strings. In this case its the object of "accounts" mentioned in routing and populated in Resolver will be available here.
 ```ts
constructor(private activatedRoute: ActivatedRoute) {}
```
make a variable *accounts* of type  *Array<Account>* to get response in it

  ```ts
accounts: Array<Account> = []
```
Implement *ngOnInit* of the account component
  
  ```ts
  ngOnInit() {
   // data property of activatedRoute is an observable to we will subscribe to it and expect it to has a property called "accounts" (as mentioned in routing) in it.
  this.activatedRoute.data.subscribe({
    next: (data) => {
      // that value will be assigned to a local variable that is used to populate the UI.
      this.accounts = data['accounts'].result;
    },
    error: (error) => {
      console.log(error);
    },
  });
}
  ```
  The *accounts* variable has all the data that is available in API. 

### **Step 6:  Creating table and printing returned data**

Create a simple table in the *account.component.html* file as shown below to show all accounts information like *title* and *currentBalance*

```html
<table width="100%" class="table table-striped table-hover">
<thead>
    <tr>
      <th width="20%">Account Title</th>
      <th width="20%">Balance</th>
    </tr>
</thead>
<tbody>
    <tr *ngFor="let account of accounts">
      <td>
        {{ account?.accountTitle }}
      </td>
      <td>
        {{ account?.currentBalance }}
      </td>
    </tr>
  </tbody>
</table>
```

Add the following classes to the *account.component.css* file for styling

```css
.table>tbody>tr>td, .table>tbody>tr>th, .table>tfoot>tr>td, .table>tfoot>tr>th, .table>thead>tr>td, .table>thead>tr>th {
    border-color: rgba(255,255,255,.1);
    padding: 12px 7px;
    vertical-align: middle;
}
  .table>tbody>tr>td, .table>thead>tr>th, .table>tfoot>tr>th {
    color: rgba(255,255,255,.7)!important;
}
 .table>thead>tr>th {
    font-size: 12px;
    text-transform: uppercase;
    font-weight: 500;
    border: 0;
}
```
### **Step 7: Create a Resolver**
Create a new typescript file to make a resolver named as *account.resolver.ts* 

```ts
import { Injectable } from "@angular/core";
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from "@angular/router";
import { EMPTY, Observable } from "rxjs";
import { catchError, first } from 'rxjs/operators';
import { Account } from "../models/account";
import AccountsService from "../services/accounts.service";

@Injectable({ providedIn: 'root' })
export class AccountResolver implements Resolve<Array<Account>> {
    constructor(private accountsService: AccountsService) { }

    resolve(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): Observable<Array<Account>> {
        return this.accountsService.getAllAccounts().pipe(
            first(), 
            catchError((err) => {console.log(err.error.responseException.exceptionMessage); return EMPTY;
          })
        );
    }
}
```
**ActivatedRouteSnapshot** contains the *information about a route associated* with a component loaded in an outlet at a particular moment in time. ActivatedRouteSnapshot can also be used to *traverse the router state tree*.

**RouterStateSnapshot** is an immutable *data structure representing the state of the router* at a particular moment in time. Any time a component is added or removed or parameter is updated, a new snapshot is created

Using **first()** because zero items emitted to be considered an error condition and *we expect at least one value from observable*, we may also use *take(1)* for the same use 

**catchError** will *handle any error* if there is any, doing above mentioned we handle it or return empty observable.

### **Step 8: Create a Route parameter for the component**

Open the routing.module file and the given import 
```ts
import { AccountResolver } from './resolver/account.resolver';
```

Add the given route to the *Routes* array with *account* as a path which will load *AccountComponent*. 

```ts
{ path: 'account', component: AccountComponent, resolve: {accounts: AccountResolver} },
```

The *resolve* property here will acts as a gateway between the API response and the route which will call the *AccountResolver* to resolve the data before loading the component

-----------
### Final output will look like this

![](/BBBank_UI/src/assets/images/2.png)









