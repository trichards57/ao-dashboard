import { createFileRoute, Link } from "@tanstack/react-router";
import validatePlace, { getPlaceDeps } from "../../support/validate-place";
import PlacePicker from "../../components/place-picker";
import {
  preloadAllVehicleSettings,
  useAllVehicleSettings,
} from "../../queries/vehicle-queries";
import { useState } from "react";
import PagePicker from "../../components/page-picker";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPen } from "@fortawesome/free-solid-svg-icons";
import { regionToString } from "../../components/region-converter";
import { useTitle } from "../../components/useTitle";
import {
  preloadDistricts,
  preloadHubs,
  Region,
} from "../../queries/place-queries";
import {
  redirectIfLoggedOut,
  redirectIfNoPermission,
} from "../../support/check-logged-in";

const PageSize = 10;

export function VehicleConfig({
  region,
  district,
  hub,
}: {
  region: Region;
  district: string;
  hub: string;
}) {
  const { data } = useAllVehicleSettings(region, district, hub);
  const [page, setPage] = useState(0);

  useTitle("Vehicle Setup");

  const itemsToDisplay = data
    .toSorted((a, b) => a.callSign.localeCompare(b.callSign))
    .slice(page * PageSize, (page + 1) * PageSize);
  const showPagination = data.length > PageSize;

  return (
    <>
      <h1 className="title">Vehicle Setup</h1>
      <PlacePicker region={region} district={district} hub={hub} />
      <div className="table-container">
        <table className="table is-striped is-fullwidth">
          <thead>
            <tr>
              <th className="edit-col"></th>
              <th className="call-sign-col">Call Sign</th>
              <th className="reg-col">Registration</th>
              <th>Region</th>
              <th>District</th>
              <th>Hub</th>
            </tr>
          </thead>
          <tbody>
            {itemsToDisplay.length > 0 &&
              itemsToDisplay.map((i) => (
                <tr key={i.id}>
                  <td className="edit-col">
                    <Link to="/vehicles/edit/$id" params={{ id: i.id }}>
                      <FontAwesomeIcon icon={faPen} />
                    </Link>
                  </td>
                  <td className="call-sign-col">{i.callSign}</td>
                  <td className="reg-col">{i.registration}</td>
                  <td className="place-col">{regionToString(i.region)}</td>
                  <td className="place-col">{i.district}</td>
                  <td className="place-col">{i.hub}</td>
                </tr>
              ))}

            {itemsToDisplay.length === 0 && (
              <tr>
                <td colSpan={5} className="no-vehicles subtitle is-6">
                  No Vehicles
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
      {showPagination && (
        <PagePicker
          page={page}
          pages={Math.ceil(data.length / PageSize)}
          setPage={setPage}
        />
      )}
    </>
  );
}

export const Route = createFileRoute("/vehicles/config")({
  validateSearch: validatePlace,
  loaderDeps: getPlaceDeps,
  loader: ({ deps, context }) =>
    Promise.all([
      preloadAllVehicleSettings(
        context.queryClient,
        deps.region,
        deps.district,
        deps.hub,
      ),
      preloadDistricts(context.queryClient, deps.region),
      preloadHubs(context.queryClient, deps.region, deps.district),
    ]),
  component: function Component() {
    return (
      <VehicleConfig
        region={Route.useSearch().region}
        district={Route.useSearch().district}
        hub={Route.useSearch().hub}
      />
    );
  },
  beforeLoad: ({ context, search }) => {
    redirectIfLoggedOut(context);
    redirectIfNoPermission(context.canEditVehicles, search);
  },
});
