import { act, render } from "@testing-library/react";
import ReactDOM from "react-dom/client";
import { Mock, beforeEach, describe, expect, it, vi } from "vitest";

import { RolePermissions, useRoles } from "../../queries/role-queries";
import { Roles } from "../../routes/roles";

vi.mock("../../queries/role-queries");

const testRoles: RolePermissions[] = [
  {
    id: "a",
    name: "Administrator",
    permissions: "Write",
    vehicleConfiguration: "Deny",
    vorData: "Deny",
  },
  {
    id: "b",
    name: "Role 1",
    permissions: "Deny",
    vehicleConfiguration: "Read",
    vorData: "Read",
  },
  {
    id: "c",
    name: "Administrator",
    permissions: "Read",
    vehicleConfiguration: "Write",
    vorData: "Write",
  },
];

describe("Roles Component", () => {
  beforeEach(() => {
    (useRoles as Mock).mockReturnValue({
      data: testRoles,
    });
  });

  it("renders without error", () => {
    const root = ReactDOM.createRoot(document.createElement("div"));
    expect(() => {
      act(() => {
        root.render(<Roles />);
        root.unmount();
      });
    }).not.toThrow();
  });

  it("renders correctly", () => {
    const { asFragment } = render(<Roles />);
    expect(asFragment()).toMatchSnapshot();
  });
});
