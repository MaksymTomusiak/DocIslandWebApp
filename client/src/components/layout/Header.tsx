const Header = () => {
  return (
    <div
      style={{
        display: "flex",
        justifyContent: "space-between",
        alignItems: "center",
        padding: "20px 20px",
        backgroundColor: "#F9FAFB",
        fontFamily: "secondary-regular",
      }}
    >
      <div className="logo" style={{ justifyContent: "center" }}>
        <img src="./header/logo.svg"></img>
      </div>
      <div
        className="menu"
        style={{
          display: "flex",
          justifyContent: "center",
          gap: "36px",
        }}
      >
        <p>Home</p>
        <p>About</p>
        <p>Resources</p>
      </div>
      <div
        className="login_button"
        style={{
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          backgroundColor: "#006D77",
          borderRadius: "6px",
          gap: "10px",
          padding: "14px 32px",
        }}
      >
        <div style={{ color: "#ffffff" }}>Login</div>
        <img style={{ color: "#ffffff" }} src="./header/arrowRight.svg" />
      </div>
    </div>
  );
};

export default Header;
