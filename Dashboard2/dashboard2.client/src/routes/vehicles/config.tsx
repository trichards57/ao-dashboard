import { createFileRoute } from "@tanstack/react-router";
import validatePlace from "../../support/validate-place";
import PlacePicker from "../../components/place-picker";
import { settingsOptions } from "../../queries/vehicle-queries";

const VehicleConfig = () => {
  return (
    <>
      <h1 className="title">Vehicle Setup</h1>
      <PlacePicker />
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
            {/* if (StatusesToDisplay.Any())
              { */}
            {/* foreach (var s in StatusesToDisplay.Skip(page * PageSize).Take(PageSize))
                  { */}
            <tr>
              <td className="edit-col">
                <a href="/vehicles/config/@s.Id">
                  <img className="fa-img" src="/img/fa/pen.svg" />
                </a>
              </td>
              <td className="call-sign-col">@s.CallSign</td>
              <td className="reg-col">@s.Registration</td>
              <td className="place-col">
                @RegionConverter.ToDisplayString(s.Region)
              </td>
              <td className="place-col">@s.District</td>
              <td className="place-col">@s.Hub</td>
            </tr>
            {/* }
              }
              else
              { */}
            <tr>
              <td colSpan={6} className="no-vehicles">
                No Vehicles
              </td>
            </tr>
            {/* } */}
          </tbody>
        </table>
      </div>
      {/* @if (ShowPagination)
  {
      <PagePicker Page="@(page)"
                  Pages="@((int)Math.Ceiling((float)StatusesToDisplay.Count()/PageSize))"
                  ChangePage="p => page = p" />
  }) */}
    </>
  );
};

export const Route = createFileRoute("/vehicles/config")({
  validateSearch: validatePlace,
  loaderDeps: ({ search: { region, district, hub } }) => ({
    region,
    district,
    hub,
  }),
  loader: ({ deps, context }) => {
    return context.queryClient.ensureQueryData(
      settingsOptions(deps.region, deps.district, deps.hub),
    );
  },
  component: VehicleConfig,
});
