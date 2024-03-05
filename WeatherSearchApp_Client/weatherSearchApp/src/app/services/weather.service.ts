import { HttpClient, HttpHeaders } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { map } from "rxjs";
import { environment } from "src/environments/environment";
import { weatherResponseModel } from "../models/weatherResponseModel";
import { WeatherSearchModel } from "../models/weatherSearchModel";
import { ServiceResponse } from "../models/serviceResponse";


@Injectable({
  providedIn: 'root',
})
export class WeatherService {
  cityRequest: WeatherSearchModel | undefined;

  constructor(private http: HttpClient) {}

  getWeatherForecastForCity(weatherSearchModel: WeatherSearchModel) {
    return this.http.post<ServiceResponse<weatherResponseModel[]>>(
    environment.apiUrl + 'Weather/get-weather-data',
    weatherSearchModel);
  }
}
