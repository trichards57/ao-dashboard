// -----------------------------------------------------------------------
// <copyright file="place-selector.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { FormControl, Grid, InputLabel, NativeSelect } from "@mui/material";
import { Dispatch } from "react";

import { useDistricts, useHubs } from "../api-hooks/places";

interface IPlaceSelector {
  place: IPlaceState;
  dispatch: Dispatch<IPlaceAction>;
}

export interface IPlaceState {
  region: string;
  district: string;
  hub: string;
}

export type IPlaceAction =
  | { type: "region"; region: string }
  | { type: "district"; district: string }
  | { type: "hub"; hub: string };

export function PlaceSelector({ place, dispatch }: Readonly<IPlaceSelector>) {
  const { data: loadedDistricts } = useDistricts(place.region);
  const districts = loadedDistricts ?? [];
  const { data: loadedHubs } = useHubs(place.region, place.district);
  const hubs = loadedHubs ?? [];

  return (
    <Grid container spacing={2}>
      <Grid item xs={12} sm={4}>
        <FormControl fullWidth>
          <InputLabel
            id="region-label"
            variant="standard"
            htmlFor="place-region"
          >
            Region
          </InputLabel>
          <NativeSelect
            id="place-region"
            value={place.region}
            onChange={(e) =>
              dispatch({ type: "region", region: e.target.value })
            }
          >
            <option value="All">All</option>
            <option value="EastOfEngland">East of England</option>
            <option value="EastMidlands">East Midlands</option>
            <option value="London">London</option>
            <option value="NorthEast">North East</option>
            <option value="NorthWest">North West</option>
            <option value="SouthEast">South East</option>
            <option value="SouthWest">South West</option>
            <option value="WestMidlands">West Midlands</option>
          </NativeSelect>
        </FormControl>
      </Grid>
      <Grid item xs={12} sm={4}>
        <FormControl fullWidth>
          <InputLabel
            id="district-label"
            variant="standard"
            htmlFor="place-district"
          >
            District
          </InputLabel>
          <NativeSelect
            id="place-district"
            value={place.district}
            disabled={districts.length === 0}
            onChange={(e) =>
              dispatch({ type: "district", district: e.target.value })
            }
          >
            <option value="All">All</option>
            {districts.map((d) => (
              <option key={d} value={d}>
                {d}
              </option>
            ))}
          </NativeSelect>
        </FormControl>
      </Grid>
      <Grid item xs={12} sm={4}>
        <FormControl fullWidth>
          <InputLabel id="hub-label" variant="standard" htmlFor="place-hub">
            Hub
          </InputLabel>
          <NativeSelect
            id="place-hub"
            value={place.hub}
            disabled={hubs.length === 0}
            onChange={(e) => dispatch({ type: "hub", hub: e.target.value })}
          >
            <option value="All">All</option>
            {hubs.map((h) => (
              <option key={h} value={h}>
                {h}
              </option>
            ))}
          </NativeSelect>
        </FormControl>
      </Grid>
    </Grid>
  );
}
