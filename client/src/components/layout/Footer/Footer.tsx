import "./footer.css";

const Footer = () => {
  return (
    <div className="footer">
      <div className="content">
        <div className="text-group">
          <div className="text-group-title">
            <img src="./header/logo.svg" alt="logo" />
            Doc Island
          </div>
          <div className="text-group-content">
            Doc Island is your go-to AI tool for unlocking insights from
            documents. Upload your PDFs, ask questions, and get instant,
            accurate answers with ease.
          </div>
        </div>

        <div className="text-group">
          <div className="text-group-title">Explore</div>
          <div className="text-group-content">
            Discover how Doc Island can help you. Learn about our features,
            pricing plans, and the benefits of using AI to manage your documents
            efficiently.
          </div>
        </div>

        <div className="text-group">
          <div className="text-group-title">Support</div>
          <div className="text-group-content">
            Need help with Doc Island? Access our FAQs, read our terms of
            service, or reach out to our support team for assistance with your
            queries.
          </div>
        </div>

        <div className="text-group">
          <div className="text-group-title">Connect</div>
          <div className="text-group-content">
            Stay connected with Doc Island. Reach out via email or follow us on
            social media for updates, tips, and more.
          </div>
        </div>
      </div>
      <div className="copyright">© 2025 Doc Island. All rights reserved.</div>
    </div>
  );
};

export default Footer;
