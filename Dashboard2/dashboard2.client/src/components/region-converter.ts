import { Region } from "../queries/place-queries";

export default function regionToString(region: Region): string {
  switch (region) {
    case "NorthEast":
      return "North East";
    case "NorthWest":
      return "North West";
    case "SouthEast":
      return "South East";
    case "SouthWest":
      return "South West";
    case "EastOfEngland":
      return "East of England";
    case "WestMidlands":
      return "West Midlands";
    case "EastMidlands":
      return "East Midlands";
    default:
      return region;
  }
}
