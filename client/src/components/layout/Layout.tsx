import { Outlet } from "react-router-dom";
import Header from "./Header";
import Footer from "./Footer/Footer";
import "./Footer/footer.css";

const Layout = () => {
  return (
    <div
      style={{
        display: "flex",
        flexDirection: "column",
        height: "100vh",
      }}
    >
      <Header />
      <Outlet />
      <Footer />
    </div>
  );
};

export default Layout;
