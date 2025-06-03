import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ClerkProvider, SignedOut } from '@clerk/clerk-react';
import Layout from '../components/layout/Layout';
import HomePage from '../features/homePage/HomePage';
import AiChatPage from '../features/ai-chat/AiChatPage';
import ChatSelectionPage from '../features/chat-selection/ChatSelectionPage';
import NotFoundPage from '../components/common/not-found/NotFoundPage';
import AdminUsersPage from '../features/admin/AdminUsersPage';
import ClerkProtectedRoute from './ClerkProtectedRoute';
import AdminProtectedRoute from '../components/auth/AdminProtectedRoute';
import BanProtectedRoute from '../components/auth/BanProtectedRoute';
import AdminStatusChecker from '../components/auth/AdminStatusChecker';
import BanStatusChecker from '../components/auth/BanStatusChecker';
import SignInPage from '../features/auth/SignInPage';
import SignUpPage from '../features/auth/SignUpPage';

const clerkKey = import.meta.env.VITE_CLERK_PUBLISHABLE_KEY;
console.log(clerkKey);
if (!clerkKey) {
    throw new Error('Missing Clerk Publishable Key');
}

const Router = () => {
    return (
        <ClerkProvider publishableKey={clerkKey}>
            <AdminStatusChecker />
            <BanStatusChecker />
            <BrowserRouter>
                <Routes>
                    <Route
                        path="/login/*"
                        element={
                            <SignedOut>
                                <SignInPage />
                            </SignedOut>
                        }
                    />
                    <Route
                        path="/sign-up"
                        element={
                            <SignedOut>
                                <SignUpPage />
                            </SignedOut>
                        }
                    />
                    <Route element={<Layout />}>
                        <Route index element={<HomePage />} />
                        <Route
                            path="/chat/:conversationId"
                            element={
                                <ClerkProtectedRoute>
                                    <BanProtectedRoute>
                                        <AiChatPage />
                                    </BanProtectedRoute>
                                </ClerkProtectedRoute>
                            }
                        />
                        <Route
                            path="/select-chat"
                            element={
                                <ClerkProtectedRoute>
                                    <BanProtectedRoute>
                                        <ChatSelectionPage />
                                    </BanProtectedRoute>
                                </ClerkProtectedRoute>
                            }
                        />
                        <Route
                            path="/admin/users"
                            element={
                                <ClerkProtectedRoute>
                                    <AdminProtectedRoute>
                                        <BanProtectedRoute>
                                            <AdminUsersPage />
                                        </BanProtectedRoute>
                                    </AdminProtectedRoute>
                                </ClerkProtectedRoute>
                            }
                        />
                        <Route
                            path="/banned"
                            element={
                                <ClerkProtectedRoute>
                                    <BanProtectedRoute>
                                        <Navigate to="/" replace />
                                    </BanProtectedRoute>
                                </ClerkProtectedRoute>
                            }
                        />
                        <Route path="*" element={<NotFoundPage />} />
                    </Route>
                </Routes>
            </BrowserRouter>
        </ClerkProvider>
    );
};

export default Router;
