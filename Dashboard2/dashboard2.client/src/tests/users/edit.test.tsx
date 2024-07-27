import { useNavigate } from "@tanstack/react-router";
import { act, render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import ReactDOM from "react-dom/client";
import { Mock, beforeEach, describe, expect, it, vi } from "vitest";

import { RolePermissions, useRoles } from "../../queries/role-queries";
import {
  UserRoleUpdate,
  UserWithRole,
  useUpdateUser,
  useUser,
} from "../../queries/user-queries";
import { EditUser } from "../../routes/users/edit.$id";

vi.mock("../../queries/role-queries");
vi.mock("../../queries/user-queries");

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
    name: "Role 2",
    permissions: "Read",
    vehicleConfiguration: "Write",
    vorData: "Write",
  },
];

const testItem: UserWithRole = {
  id: "d",
  name: "User1",
  role: "Role 1",
  roleId: "b",
};

const testUpdate: UserRoleUpdate = {
  roleId: "c",
};

describe("Edit User Page", () => {
  beforeEach(() => {
    (useRoles as Mock).mockReturnValue({
      isLoading: false,
      data: testRoles,
    });
    (useUser as Mock).mockReturnValue({
      isLoading: false,
      data: testItem,
    });
    (useUpdateUser as Mock).mockReturnValue({
      mutateAsync: vi.fn(),
    });
  });

  it("renders without error", () => {
    const root = ReactDOM.createRoot(document.createElement("div"));
    expect(() => {
      act(() => {
        root.render(<EditUser id={testItem.id} isAdmin />);
        root.unmount();
      });
    }).not.toThrow();
  });

  it("renders correctly", () => {
    const { asFragment } = render(<EditUser id={testItem.id} />);
    expect(asFragment()).toMatchSnapshot();

    expect(screen.getByLabelText(/role/i)).toHaveValue(testItem.roleId);

    expect(useUser).toBeCalledWith(testItem.id);
  });

  it("renders admin correctly", () => {
    const { asFragment } = render(<EditUser id={testItem.id} isAdmin />);
    expect(asFragment()).toMatchSnapshot();

    expect(screen.getByLabelText(/role/i)).toHaveValue(testItem.roleId);
  });

  it("renders none correctly", () => {
    (useUser as Mock).mockReturnValue({
      isLoading: false,
      data: { ...testItem, roleId: "None" },
    });

    const { asFragment } = render(<EditUser id={testItem.id} isAdmin />);
    expect(asFragment()).toMatchSnapshot();

    expect(screen.getByLabelText(/role/i)).toHaveValue("None");
  });

  it("submits details to update api", async () => {
    const user = userEvent.setup();
    const mutateFn = vi.fn();
    const navigateFn = vi.fn();

    (useNavigate as Mock).mockReturnValue(navigateFn);
    (useUpdateUser as Mock).mockReturnValue({
      mutateAsync: mutateFn,
    });

    render(<EditUser id={testItem.id} />);

    await user.selectOptions(screen.getByLabelText(/role/i), testUpdate.roleId);

    await user.click(screen.getByRole("button", { name: /save/i }));

    expect(screen.getByLabelText(/role/i)).toBeValid();
    expect(mutateFn).toBeCalledWith(testUpdate);
    expect(navigateFn).toBeCalledWith({
      to: "/users",
    });
  });

  it("disabled items when running", async () => {
    const user = userEvent.setup();
    const mutateFn = vi.fn().mockReturnValue(new Promise(() => {}));
    const navigateFn = vi.fn();

    (useNavigate as Mock).mockReturnValue(navigateFn);
    (useUpdateUser as Mock).mockReturnValue({
      mutateAsync: mutateFn,
    });

    render(<EditUser id={testItem.id} />);

    await user.selectOptions(screen.getByLabelText(/role/i), testUpdate.roleId);

    await user.click(screen.getByRole("button", { name: /save/i }));

    expect(screen.getByLabelText(/role/i)).toBeDisabled();
    expect(screen.getByRole("button", { name: /save/i })).toBeDisabled();
  });
});
