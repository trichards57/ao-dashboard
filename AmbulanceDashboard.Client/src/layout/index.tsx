// -----------------------------------------------------------------------
// <copyright file="index.ts" company="Tony Richards">
// Copyright (c) Tony Richards. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for
// full license information.
// </copyright>
// -----------------------------------------------------------------------

import { AuthenticatedTemplate, useMsal } from "@azure/msal-react";
import {
  CarCrash as CarCrashIcon,
  ChevronLeft as ChevronLeftIcon,
  Home as HomeIcon,
  Logout as LogoutIcon,
  Menu as MenuIcon,
  Settings as SettingsIcon,
} from "@mui/icons-material";
import {
  Box,
  Container,
  Divider,
  IconButton,
  Link,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  AppBar as MuiAppBar,
  AppBarProps as MuiAppBarProps,
  Drawer as MuiDrawer,
  Toolbar,
  Typography,
} from "@mui/material";
import { styled } from "@mui/material/styles";
import { LinkProps, Link as NavLink } from "@tanstack/react-router";
import {
  PropsWithChildren,
  forwardRef,
  useState,
} from "react";

import { useUserPermissions } from "../api-hooks/users";

function Copyright() {
  return (
    <Typography
      variant="body2"
      color="text.secondary"
      align="center"
      sx={{ pt: 4 }}
    >
      {"Copyright © "}
      <Link color="inherit" href="https://tr-toolbox.me.uk/" target="_blank" rel="noreferrer">
        Tony Richards
      </Link>
      {" "}
      {new Date().getFullYear()}
      , all rights reserved.
      {" "}
      <Link color="inherit" href="https://www.flaticon.com/free-icons/car" target="_blank" rel="noreferrer">Ambulance logo created by Freepik - Flaticon</Link>
    </Typography>
  );
}

const drawerWidth: number = 240;

interface AppBarProps extends MuiAppBarProps {
  open?: boolean;
}

const AppBar = styled(MuiAppBar, {
  shouldForwardProp: (prop) => prop !== "open",
})<AppBarProps>(({ theme, open }) => ({
  zIndex: theme.zIndex.drawer + 1,
  transition: theme.transitions.create(["width", "margin"], {
    easing: theme.transitions.easing.sharp,
    duration: theme.transitions.duration.leavingScreen,
  }),
  ...(open && {
    marginLeft: drawerWidth,
    width: `calc(100% - ${drawerWidth}px)`,
    transition: theme.transitions.create(["width", "margin"], {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.enteringScreen,
    }),
  }),
}));

const Drawer = styled(MuiDrawer, {
  shouldForwardProp: (prop) => prop !== "open",
})(({ theme, open }) => ({
  "& .MuiDrawer-paper": {
    position: "relative",
    whiteSpace: "nowrap",
    width: drawerWidth,
    transition: theme.transitions.create("width", {
      easing: theme.transitions.easing.sharp,
      duration: theme.transitions.duration.enteringScreen,
    }),
    boxSizing: "border-box",
    ...(!open && {
      overflowX: "hidden",
      transition: theme.transitions.create("width", {
        easing: theme.transitions.easing.sharp,
        duration: theme.transitions.duration.leavingScreen,
      }),
      width: theme.spacing(7),
      [theme.breakpoints.up("sm")]: {
        width: theme.spacing(9),
      },
    }),
  },
}));

const LocalNavLink = forwardRef<HTMLAnchorElement, LinkProps & { href: string }>(
  // eslint-disable-next-line react/jsx-props-no-spreading
  ({ href, ...props }, ref) => <NavLink ref={ref} {...props} to={href} />,
);

export default function Layout({ children }: Readonly<PropsWithChildren>) {
  const { data: permissions } = useUserPermissions();
  const [open, setOpen] = useState(false);

  const toggleDrawer = () => {
    setOpen(!open);
  };

  const { instance } = useMsal();

  const signOut = () => {
    instance.logoutRedirect();
  };

  return (
    <Box sx={{ display: "flex" }}>
      <AppBar position="absolute" open={open}>
        <Toolbar
          sx={{
            pr: "24px", // keep right padding when drawer closed
          }}
        >
          <IconButton
            edge="start"
            color="inherit"
            aria-label="open drawer"
            onClick={toggleDrawer}
            sx={{
              marginRight: "36px",
              ...(open && { display: "none" }),
            }}
          >
            <MenuIcon />
          </IconButton>
          <Typography
            component="h1"
            variant="h6"
            color="inherit"
            noWrap
            sx={{ flexGrow: 1 }}
          >
            Ambulance Dashboard
          </Typography>
          <Typography
            component="p"
            variant="body1"
            color="inherit"
            noWrap
          >
            {permissions?.userId}
          </Typography>
        </Toolbar>
      </AppBar>
      <Drawer variant="permanent" open={open}>
        <Toolbar
          sx={{
            display: "flex",
            alignItems: "center",
            justifyContent: "flex-end",
            px: [1],
          }}
        >
          <IconButton onClick={toggleDrawer}>
            <ChevronLeftIcon />
          </IconButton>
        </Toolbar>
        <Divider />
        <List component="nav">
          <ListItemButton LinkComponent={LocalNavLink} href="/home">
            <ListItemIcon>
              <HomeIcon />
            </ListItemIcon>
            <ListItemText primary="Home" />
          </ListItemButton>
          <AuthenticatedTemplate>
            <ListItemButton
              LinkComponent={LocalNavLink}
              href="/vehicles/status"
            >
              <ListItemIcon>
                <CarCrashIcon />
              </ListItemIcon>
              <ListItemText primary="Vehicle Status" />
            </ListItemButton>
            {permissions?.canEditVehicles && (
              <ListItemButton
                LinkComponent={LocalNavLink}
                href="/vehicles/config"
              >
                <ListItemIcon>
                  <SettingsIcon />
                </ListItemIcon>
                <ListItemText primary="Vehicle Configuration" />
              </ListItemButton>
            )}
            <Divider />
            <ListItemButton onClick={() => signOut()}>
              <ListItemIcon>
                <LogoutIcon />
              </ListItemIcon>
              <ListItemText primary="Log Out" />
            </ListItemButton>
          </AuthenticatedTemplate>
        </List>
      </Drawer>
      <Box
        component="main"
        sx={{
          backgroundColor: (theme) => (theme.palette.mode === "light"
            ? theme.palette.grey[100]
            : theme.palette.grey[900]),
          flexGrow: 1,
          height: "100vh",
          overflow: "auto",
        }}
      >
        <Toolbar />
        <Container maxWidth="lg" sx={{ mt: 4, mb: 4 }}>
          {children}
          <Copyright />
        </Container>
      </Box>
    </Box>
  );
}
