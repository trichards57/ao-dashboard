import { Link } from "react-router-dom";
import useRequireLoggedIn from "../hooks/useRequireLoggedIn";

export default function Home() {
  useRequireLoggedIn();

  return (
    <div className="card">
      <h1>Home</h1>
      <a aria-disabled="true" className="button" href="/vehicles/status">
        VOR Status
      </a>
      <a aria-disabled="true" className="button" href="/vehicles/upcoming">
        Upcoming Work
      </a>
      <a aria-disabled="true" className="button" href="/vehicles/capacity">
        Capacity and Travel Warnings
      </a>
      <Link className="button" to="/vehicles/config">
        Configure Vehicles
      </Link>
    </div>
  );
}
