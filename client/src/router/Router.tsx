import { BrowserRouter, Routes, Route } from "react-router-dom";
import NotFoundPage from "../components/common/NotFoundPage";
import Layout from "../components/layout/Layout";
import HomePage from "../features/homePage/HomePage";

const Router = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="*" element={<NotFoundPage />} />
        <Route path="/" element={<Layout />}>
          <Route path="/" element={<HomePage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
};

export default Router;
