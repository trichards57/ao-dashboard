import { AlertColor, FormControl, Grid, InputLabel, MenuItem, Select } from "@mui/material";
import { useEffect, useState } from "react";
import { useDistricts, useHubs } from "./hooks";

interface IPlaceSelector {
  onPlaceChanged: (region: string, district: string, hub: string) => void;
  onError: (message: string, severity: AlertColor) => void;
}

export default function PlaceSelector({ onPlaceChanged, onError }: IPlaceSelector) {
  const [selectedRegion, setSelectedRegion] = useState("All");
  const [selectedDistrict, setSelectedDistrict] = useState("All");
  const [selectedHub, setSelectedHub] = useState("All");

  const districts = useDistricts(selectedRegion, onError);
  const hubs = useHubs(selectedRegion, selectedDistrict, onError);

  useEffect(() => {
    onPlaceChanged(selectedRegion, selectedDistrict, selectedHub);
  }, [onPlaceChanged, selectedRegion, selectedDistrict, selectedHub]);

  return (
    <Grid container spacing={2}>
      <Grid item xs={12} sm={4}>
        <FormControl fullWidth>
          <InputLabel id="region-label">Region</InputLabel>
          <Select
            label="Region"
            labelId="region-label"
            value={selectedRegion}
            onChange={(e) => setSelectedRegion(e.target.value)}
          >
            <MenuItem value="All">All</MenuItem>
            <MenuItem value="EastOfEngland">East of England</MenuItem>
            <MenuItem value="EastMidlands">East Midlands</MenuItem>
            <MenuItem value="London">London</MenuItem>
            <MenuItem value="NorthEast">North East</MenuItem>
            <MenuItem value="NorthWest">North West</MenuItem>
            <MenuItem value="SouthEast">South East</MenuItem>
            <MenuItem value="SouthWest">South West</MenuItem>
            <MenuItem value="WestMidlands">West Midlands</MenuItem>
          </Select>
        </FormControl>
      </Grid>
      <Grid item xs={12} sm={4}>
        <FormControl fullWidth>
          <InputLabel id="district-label">District</InputLabel>
          <Select
            label="District"
            labelId="district-label"
            value={selectedDistrict}
            disabled={districts.length === 0}
            onChange={(e) => setSelectedDistrict(e.target.value)}
          >
            <MenuItem value="All">All</MenuItem>
            {districts.map((d) => (
              <MenuItem value={d}>{d}</MenuItem>
            ))}
          </Select>
        </FormControl>
      </Grid>
      <Grid item xs={12} sm={4}>
        <FormControl fullWidth>
          <InputLabel id="hub-label">Hub</InputLabel>
          <Select
            label="Hub"
            labelId="hub-label"
            value={selectedHub}
            disabled={hubs.length === 0}
            onChange={(e) => setSelectedHub(e.target.value)}
          >
            <MenuItem value="All">All</MenuItem>
            {hubs.map((h) => (
              <MenuItem value={h}>{h}</MenuItem>
            ))}
          </Select>
        </FormControl>
      </Grid>
    </Grid>
  );
}
