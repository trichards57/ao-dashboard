import { styled } from "@mui/material/styles";
import {
  Typography,
  Link,
  Box,
  Toolbar,
  IconButton,
  Divider,
  List,
  Container,
  AppBar as MuiAppBar,
  AppBarProps as MuiAppBarProps,
  Drawer as MuiDrawer,
  ListItemButton,
  ListItemIcon,
  ListItemText,
} from "@mui/material";
import {
  CarCrash as CarCrashIcon,
  Settings as SettingsIcon,
  Home as HomeIcon,
  Menu as MenuIcon,
  ChevronLeft as ChevronLeftIcon,
  Logout as LogoutIcon,
} from "@mui/icons-material";
import {
  PropsWithChildren,
  forwardRef,
  useCallback,
  useMemo,
  useState,
} from "react";
import {
  Location,
  NavLink,
  NavLinkProps,
  ScrollRestoration,
} from "react-router-dom";
import { AuthenticatedTemplate, useMsal } from "@azure/msal-react";

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
      </Link>{" "}
      {new Date().getFullYear()}
      , all rights reserved.  <Link color="inherit" href="https://www.flaticon.com/free-icons/car" target="_blank" rel="noreferrer">Ambulance logo created by Freepik - Flaticon</Link>
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

export default function Layout({ children }: PropsWithChildren) {
  const [open, setOpen] = useState(false);
  const getKey = useCallback((l: Location) => l.key, []);

  const toggleDrawer = () => {
    setOpen(!open);
  };

  const { instance } = useMsal();

  const signOut = () => {
    instance.logoutRedirect();
  };

  const LocalNavLink = useMemo(
    () =>
      forwardRef<HTMLAnchorElement, NavLinkProps & { href: string }>(
        ({ href, ...props }, ref) => <NavLink ref={ref} {...props} to={href} />
      ),
    []
  );

  return (
    <>
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
            <ListItemButton LinkComponent={LocalNavLink} href="/">
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
              <ListItemButton
                LinkComponent={LocalNavLink}
                href="/vehicles/config"
              >
                <ListItemIcon>
                  <SettingsIcon />
                </ListItemIcon>
                <ListItemText primary="Vehicle Configuration" />
              </ListItemButton>
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
            backgroundColor: (theme) =>
              theme.palette.mode === "light"
                ? theme.palette.grey[100]
                : theme.palette.grey[900],
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
      <ScrollRestoration getKey={getKey} />
    </>
  );
}
