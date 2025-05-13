import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { ClerkProvider, SignedOut } from '@clerk/clerk-react';
import Layout from '../components/layout/Layout';
import NotFoundPage from '../components/common/NotFoundPage';
import HomePage from '../features/homePage/HomePage';
import SignInPage from '../features/auth/sign-in/SignInPage';
import AiChatPage from '../features/ai-chat/AiChatPage';
import ChatSelectionPage from '../features/chat-selection/ChatSelectionPage';
import ClerkProtectedRoute from './ClerkProtectedRoute';

const clerkKey = process.env.VITE_PUBLIC_CLERK_PUBLISHABLE_KEY;

if (!clerkKey) {
    throw new Error('Missing Clerk Publishable Key');
}

const Router = () => {
    return (
        <ClerkProvider publishableKey={clerkKey}>
            <BrowserRouter>
                <Routes>
                    <Route
                        path="/login"
                        element={
                            <SignedOut>
                                <SignInPage />
                            </SignedOut>
                        }
                    />
                    <Route path="/" element={<Layout />}>
                        <Route index element={<HomePage />} />
                        <Route
                            path="/select-chat"
                            element={
                                <ClerkProtectedRoute>
                                    <ChatSelectionPage />
                                </ClerkProtectedRoute>
                            }
                        />
                        <Route
                            path="/chat/:chatId"
                            element={
                                <ClerkProtectedRoute>
                                    <AiChatPage />
                                </ClerkProtectedRoute>
                            }
                        />
                    </Route>
                    <Route path="*" element={<NotFoundPage />} />
                </Routes>
            </BrowserRouter>
        </ClerkProvider>
    );
};

export default Router;
