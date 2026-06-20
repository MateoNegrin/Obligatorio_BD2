import { initializeApp } from "https://www.gstatic.com/firebasejs/10.5.0/firebase-app.js";
import { 
    getAuth, 
    signInWithEmailAndPassword, 
    createUserWithEmailAndPassword,
    signOut,
    onAuthStateChanged,
    updateProfile
} from "https://www.gstatic.com/firebasejs/10.5.0/firebase-auth.js";

// Configuración de Firebase (será inyectada por el servidor)
let app;
let auth;

// Esperar a que Firebase esté listo (útil cuando se llama antes de inicialización)
async function waitForReady(maxWaitMs = 10000) {
    const startTime = Date.now();
    while (!app && (Date.now() - startTime) < maxWaitMs) {
        await new Promise(resolve => setTimeout(resolve, 100));
    }
    
    if (!app) {
        console.error("✗ Firebase no se inicializó después de esperar");
        return false;
    }
    
    console.log("✓ Firebase está listo para usar");
    return true;
}

// Inicializar Firebase
async function initializeFirebase(config) {
    try {
        // Si no se pasa config, usar la del window
        const finalConfig = config || window.firebaseConfig;
        
        if (!finalConfig) {
            console.error("✗ No se encontró configuración de Firebase. Config:", config, "Window config:", window.firebaseConfig);
            return false;
        }
        
        console.log("Config recibida:", finalConfig);
        console.log("API Key:", finalConfig?.apiKey);
        console.log("Auth Domain:", finalConfig?.authDomain);
        
        app = initializeApp(finalConfig);
        auth = getAuth(app);
        console.log("✓ Firebase inicializado correctamente");
        
        // Escuchar cambios de autenticación
        onAuthStateChanged(auth, (user) => {
            if (user) {
                console.log("Usuario autenticado:", user.email);
            } else {
                console.log("Usuario no autenticado");
            }
        });
        
        return true;
    } catch (error) {
        console.error("✗ Error al inicializar Firebase:", error);
        return false;
    }
}

// Login con email y contraseña
async function login(email, password) {
    try {
        const userCredential = await signInWithEmailAndPassword(auth, email, password);
        const idToken = await userCredential.user.getIdToken();
        
        return {
            success: true,
            idToken: idToken,
            userId: userCredential.user.uid,
            errorMessage: null
        };
    } catch (error) {
        console.error("Error en login:", error);
        return {
            success: false,
            idToken: null,
            userId: null,
            errorMessage: getFirebaseErrorMessage(error.code)
        };
    }
}

// Registro con email y contraseña
async function signup(email, password, displayName) {
    try {
        const userCredential = await createUserWithEmailAndPassword(auth, email, password);
        
        // Actualizar el perfil con el nombre
        if (displayName) {
            await updateProfile(userCredential.user, { displayName: displayName });
        }
        
        const idToken = await userCredential.user.getIdToken();
        
        return {
            success: true,
            idToken: idToken,
            userId: userCredential.user.uid,
            errorMessage: null
        };
    } catch (error) {
        console.error("Error en signup:", error);
        return {
            success: false,
            idToken: null,
            userId: null,
            errorMessage: getFirebaseErrorMessage(error.code)
        };
    }
}

// Logout
async function logout() {
    try {
        await signOut(auth);
        console.log("Sesión cerrada");
        return true;
    } catch (error) {
        console.error("Error al logout:", error);
        return false;
    }
}

// Obtener token actual
async function getCurrentToken() {
    try {
        if (!auth) {
            console.warn("⚠ Firebase aún no está inicializado, no se puede obtener token");
            return null;
        }
        if (auth.currentUser) {
            return await auth.currentUser.getIdToken();
        }
        return null;
    } catch (error) {
        console.error("Error obteniendo token:", error);
        return null;
    }
}

// Obtener usuario actual
function getCurrentUser() {
    if (!auth) {
        console.warn("⚠ Firebase aún no está inicializado");
        return null;
    }
    return auth.currentUser;
}

// Mapear códigos de error de Firebase a mensajes legibles
function getFirebaseErrorMessage(code) {
    const errorMessages = {
        "auth/email-already-in-use": "Este email ya está registrado",
        "auth/invalid-email": "El email no es válido",
        "auth/operation-not-allowed": "Esta operación no está permitida",
        "auth/weak-password": "La contraseña es muy débil",
        "auth/user-disabled": "Este usuario ha sido deshabilitado",
        "auth/user-not-found": "Usuario no encontrado",
        "auth/wrong-password": "Contraseña incorrecta",
        "auth/invalid-credential": "Las credenciales son inválidas",
        "auth/too-many-requests": "Demasiados intentos de login. Intenta más tarde"
    };
    
    return errorMessages[code] || "Error de autenticación. Intenta nuevamente.";
}

// Exponer las funciones globalmente
window.firebaseAuth = {
    initialize: initializeFirebase,
    waitForReady: waitForReady,
    login: login,
    signup: signup,
    logout: logout,
    getCurrentToken: getCurrentToken,
    getCurrentUser: getCurrentUser
};

console.log("✓ Firebase Auth module cargado");

// No auto-inicializar aquí - Blazor lo hará desde FirebaseInitializer.razor
console.log("Esperando inicialización desde Blazor...");