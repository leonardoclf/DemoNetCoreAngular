import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  // obj default = null
  model: any = {}
  // boolean default = false 
  
  
  constructor(public accountService: AccountService) { }

  ngOnInit(): void { }

  login() {
    this.accountService.login(this.model).subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  logout() {
    this.accountService.logout();
  }
}

/*
  Observable is lazy -> need to subscribe 
  new standard for manangin async data included in ES7
  Intro in angular v2
  observables are lazy collections of multiple values over time
  observables like a newsletter
    only subscribers of the newsletter receive the newsletter
    if no-one subscribes to the newsletter it probably will not be printed
  
  Promiveses vs                    Observables
  provides a single future value x emits multiple values over time
  not lazy                       x lazy
  can not cancel                 x able to cancel
                                 x can use with map/filter/reduce/operator
  Observables and RxJS
    chain a pipe to manipulate the data that is coming back from server
  
  Getting data from observables
    .subscribe(next(), ifError(), whenComplete())
  
  Async pipe
    " | async" -> auto subscribes / unsubscribes from observable

  !!user - double exclamantion mark 
    if user isnt null == true
    else false
  
  now that is subscribed -> its persist when refresh
*/