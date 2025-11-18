//this file contins invironment variables

// Auto environment: use localhost if running locally, otherwise production
const isLocal = typeof window !== "undefined" && (window.location.hostname === "localhost" || window.location.hostname === "127.0.0.1");
const API_BASE_URL = isLocal ? "http://localhost:5042/api/" : "https://gomango01-001-site1.mtempurl.com/api/";
export const ServerPath = isLocal ? "http://localhost:5042" : "https://gomango01-001-site1.mtempurl.com";
export const NetlifyDomain = "https://souq-elbalad.netlify.app/";
export const SiteName = "Gomango";
export default API_BASE_URL;
