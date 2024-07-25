import { useNavigate } from "@tanstack/react-router";
import { Region, useDistricts, useHubs } from "../queries/place-queries";

function calculateSearch(region: Region, district: string, hub: string) {
  if (region?.toUpperCase() === "ALL") {
    return {};
  }
  if (district?.toUpperCase() === "ALL") {
    return { region };
  }
  if (hub?.toUpperCase() === "ALL") {
    return { region, district };
  }

  return {
    region,
    district,
    hub,
  };
}

export default function PlacePicker({
  region,
  district,
  hub,
}: {
  region: Region;
  district: string;
  hub: string;
}) {
  const navigate = useNavigate();
  const { data: districts } = useDistricts(region);
  const { data: hubs } = useHubs(region, district);

  return (
    <div className="columns is-desktop">
      <div className="column">
        <div className="field">
          <label className="label" htmlFor="region">
            Region
          </label>
          <div className="select is-fullwidth">
            <select
              id="region"
              onChange={(e) => {
                navigate({
                  search: calculateSearch(
                    e.target.value as Region,
                    district,
                    hub,
                  ),
                });
              }}
              value={region ?? "All"}
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
            </select>
          </div>
        </div>
      </div>
      <div className="column">
        <div className="field">
          <label className="label" htmlFor="district">
            District
          </label>
          <div className="select is-fullwidth">
            <select
              id="region"
              value={district ?? "All"}
              disabled={(districts ?? []).length === 0}
              onChange={(e) => {
                navigate({
                  search: calculateSearch(region, e.target.value, hub),
                });
              }}
            >
              <option value="All">All</option>
              {districts?.map((d) => (
                <option value={d} key={d}>
                  {d}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>
      <div className="column">
        <div className="field">
          <label className="label" htmlFor="hub">
            Hub
          </label>
          <div className="select is-fullwidth">
            <select
              id="hub"
              value={hub ?? "all"}
              disabled={(hubs ?? []).length === 0}
              onChange={(e) => {
                navigate({
                  search: calculateSearch(region, district, e.target.value),
                });
              }}
            >
              <option value="all">All</option>
              {hubs?.map((h) => (
                <option value={h} key={h}>
                  {h}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>
    </div>
  );
}
