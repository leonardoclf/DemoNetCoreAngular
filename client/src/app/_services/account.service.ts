import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from "rxjs/operators";
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = 'https://localhost:5001/api/';
  sizeOfBuffer = 1;
  private currentUserSource = new ReplaySubject<User>(this.sizeOfBuffer);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'Account/login', model).pipe(
        map((response: User) => {
          const user = response;
          if (user) {
            localStorage.setItem('user', JSON.stringify(user));
            this.currentUserSource.next(user);
          }
        })
    )
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'Account/register', model).pipe(
      map((user: User) => {
        if (user) {
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
        }
        return user; 
      })
    )
  }

  setCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }
}

/*
  Service is a singleton - service lifecycle all along the app is open
  post require something in the body

  Everthing inside the pipe is RxJS operator

  ReplaySubject - kind of buffer object
*/