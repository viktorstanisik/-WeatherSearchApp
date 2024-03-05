import { Component, OnInit } from '@angular/core';
import { Route, Router } from '@angular/router';
import { AccountService } from './services/account.service';
import { AuthGuard } from './_guards/auth.guard';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'WeatherApp';
  constructor(public accountService: AccountService, private router: Router, private authGuard: AuthGuard) {}
  ngOnInit(): void {
    this.accountService.isUserAuthenticated().subscribe(authenticated => {
      if (authenticated) {
        const canActivate = this.authGuard.canActivate();
        if (canActivate) {
          //get the currentRoute in order to know where to navigate
          const routeName = this.getCurrentRouteName();
          //Check what is the entered route if the entered route is not from router config or is get weather route the user to default (get weather)
          //if the route is some route from predefined routes than route to that one
          if (routeName && routeName !== '' &&
              routeName !== 'getweather' &&
              this.getPredefinedRoutes().includes(routeName)) {

            this.router.navigate([routeName]);

          } else {
            this.router.navigateByUrl('getweather')
          }
          this.accountService.setCurrentUser(true);
        }
      }
    });
  }

  getCurrentRouteName(): string {
    const currentUrl = this.router.url;
    const routeParts = currentUrl.split('/'); // Split the URL by '/'
    return routeParts[routeParts.length - 1]; // Get the last segment as the route name
  }
  getPredefinedRoutes(): (string | undefined)[]  {
    const routes: Route[] = this.router.config;
    const predefinedRoutes = routes.map(route => route.path);
    return  predefinedRoutes.filter(route => route !== '**' && route !== ''); // Remove wildcard and empty routes
  }
}
