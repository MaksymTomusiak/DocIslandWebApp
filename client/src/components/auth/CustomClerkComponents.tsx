import { SignIn, SignUp, UserButton } from '@clerk/clerk-react';
import './custom-clerk-components.css';

const clerkAppearance = {
    elements: {
        rootBox: 'custom-clerk-root',
        card: 'custom-clerk-card',
        headerTitle: 'custom-clerk-header-title',
        headerSubtitle: 'custom-clerk-header-subtitle',
        socialButtonsBlockButton: 'custom-clerk-social-button',
        formButtonPrimary: 'custom-clerk-primary-button',
        formFieldInput: 'custom-clerk-input',
        formFieldLabel: 'custom-clerk-label',
        footerActionLink: 'custom-clerk-footer-link',
        identityPreviewEditButton: 'custom-clerk-edit-button',
        formFieldAction: 'custom-clerk-form-action',
        alert: 'custom-clerk-alert',
        alertText: 'custom-clerk-alert-text',
        formFieldWarningText: 'custom-clerk-warning-text',
        formFieldInputShowPasswordButton: 'custom-clerk-show-password-button',
        otpCodeFieldInput: 'custom-clerk-otp-input',
        userPreview: 'custom-clerk-user-preview',
        userButtonPopoverCard: 'custom-clerk-popover-card',
        userButtonPopoverActionButton: 'custom-clerk-popover-button',
        userButtonPopoverFooter: 'custom-clerk-popover-footer',
        userButtonPopoverActionButtonIcon: 'custom-clerk-popover-icon',
        userButtonPopoverActionButtonText: 'custom-clerk-popover-text',
    },
};

export const CustomSignIn = () => {
    return (
        <div className="custom-clerk-container">
            <SignIn appearance={clerkAppearance} />
        </div>
    );
};

export const CustomSignUp = () => {
    return (
        <div className="custom-clerk-container">
            <SignUp appearance={clerkAppearance} />
        </div>
    );
};

export const CustomUserButton = () => {
    return <UserButton appearance={clerkAppearance} />;
};
