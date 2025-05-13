import { SignIn } from '@clerk/react-router';
import './sign-in.css';
export default function SignInPage() {
    return (
        <div className="sign-in-container">
            <SignIn />
        </div>
    );
}
