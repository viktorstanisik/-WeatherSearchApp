import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { finalize, tap } from 'rxjs';
import { ErrorMessages } from 'src/app/helpers/errorMessages';
import { weatherResponseModel } from 'src/app/models/weatherResponseModel';
import { WeatherSearchModel } from 'src/app/models/weatherSearchModel';
import { WeatherService } from 'src/app/services/weather.service';

@Component({
  selector: 'app-get-weather',
  templateUrl: './get-weather.component.html',
  styleUrls: ['./get-weather.component.css']
})
export class GetWeatherComponent implements OnInit {

  model: WeatherSearchModel = {cityName: ''};
  cities: weatherResponseModel[] | undefined;
  showSpinner: boolean = false;

  constructor(private weatherService: WeatherService, private toastr: ToastrService) { }


  ngOnInit(): void {
  }

  getWeather() {
    this.showSpinner = true;
    this.weatherService.getWeatherForecastForCity(this.model)
      .pipe(
        tap(response => {
          if (response.success) {
            this.cities = response.data.map(weatherResponse => weatherResponse);
          } else {
            const errorMessage = response.errorMessage || ErrorMessages.genericUnknownError;
            this.toastr.error(errorMessage, 'Error');
          }
        }),
        finalize(() => {
          this.showSpinner = false;
        })
      )
      .subscribe();
  }
}
