import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { GetWeatherComponent } from './components/get-weather/get-weather.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { CreatedaccountComponent } from './components/createdaccount/createdaccount.component';
import { CommonModule } from '@angular/common';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ToastrModule } from 'ngx-toastr';
import { AuthGuard } from './_guards/auth.guard';
import { AccountInfoComponent } from './components/account-info/account-info.component';


const routes:  Routes = [
  {path: '', component: LoginComponent},
  {path: 'register', component: RegisterComponent},
  {path: 'getweather', component:GetWeatherComponent, canActivate: [AuthGuard]},
  {path: 'account-information', component:AccountInfoComponent, canActivate: [AuthGuard]},
  {path: 'account-created', component:CreatedaccountComponent},
  {path: '**', component: LoginComponent, pathMatch: 'full'},
];

@NgModule({
  imports: [CommonModule, BrowserAnimationsModule,ToastrModule.forRoot(), RouterModule.forRoot(routes)],
  exports: [RouterModule, FormsModule],
})
export class AppRoutingModule { }
