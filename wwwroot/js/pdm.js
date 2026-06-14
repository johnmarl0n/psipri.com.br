/* ==========================================================================
   Pingo de Mel (PDM) — Frontend Logic
   ========================================================================== */

document.addEventListener("DOMContentLoaded", () => {
    // --- Responsive Sidebar Toggle ---
    const hamburger = document.getElementById("pdmHamburger");
    const sidebar   = document.getElementById("pdmSidebar");
    const overlay   = document.getElementById("pdmOverlay");

    function openSidebar() {
        sidebar.classList.add("open");
        overlay.classList.add("active");
        document.body.style.overflow = "hidden"; // Prevent background scroll
        hamburger.setAttribute("aria-expanded", "true");
        hamburger.querySelector("i").className = "fas fa-times"; // X icon
    }

    function closeSidebar() {
        sidebar.classList.remove("open");
        overlay.classList.remove("active");
        document.body.style.overflow = "";
        hamburger.setAttribute("aria-expanded", "false");
        hamburger.querySelector("i").className = "fas fa-bars";
    }

    if (hamburger && sidebar && overlay) {
        hamburger.addEventListener("click", (e) => {
            e.stopPropagation();
            sidebar.classList.contains("open") ? closeSidebar() : openSidebar();
        });

        // Close when clicking the overlay
        overlay.addEventListener("click", closeSidebar);

        // Auto-close when a nav link is clicked (critical for mobile usability)
        sidebar.querySelectorAll("a").forEach(link => {
            link.addEventListener("click", () => {
                if (window.innerWidth <= 1024) {
                    closeSidebar();
                }
            });
        });
    }

    // --- Auto-calculate Product Selling Price ---
    const precoCustoInput = document.getElementById("PrecoCusto");
    const margemInput = document.getElementById("Margem");
    const precoVendaInput = document.getElementById("PrecoVendaCalculado");

    function updatePrecoVenda() {
        if (precoCustoInput && margemInput && precoVendaInput) {
            const custo = parseFloat(precoCustoInput.value) || 0;
            const margem = parseFloat(margemInput.value) || 0;
            const venda = custo * (1 + margem / 100);
            precoVendaInput.value = venda.toLocaleString("pt-BR", { style: "currency", currency: "BRL" });
        }
    }

    if (precoCustoInput && margemInput) {
        precoCustoInput.addEventListener("input", updatePrecoVenda);
        margemInput.addEventListener("input", updatePrecoVenda);
        updatePrecoVenda(); // Run initially
    }

    // --- Image Preview on Upload ---
    const imageInput = document.getElementById("imageFileInput");
    const previewImg = document.getElementById("previewImage");
    const placeholder = document.getElementById("previewPlaceholder");

    if (imageInput) {
        imageInput.addEventListener("change", function() {
            const file = this.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function(e) {
                    if (previewImg) {
                        previewImg.src = e.target.result;
                        previewImg.style.display = "block";
                    }
                    if (placeholder) {
                        placeholder.style.display = "none";
                    }
                };
                reader.readAsDataURL(file);
            }
        });
    }
});

// Toast/notification utility
function showToast(message, type = "success") {
    const alertDiv = document.createElement("div");
    alertDiv.className = `pdm-alert-box pdm-alert-${type} fade-in`;
    alertDiv.style.position = "fixed";
    alertDiv.style.bottom = "20px";
    alertDiv.style.right = "20px";
    alertDiv.style.zIndex = "9999";
    alertDiv.style.boxShadow = "0 10px 25px rgba(0,0,0,0.1)";
    alertDiv.innerHTML = `
        <i class="fas ${type === 'success' ? 'fa-check-circle' : type === 'danger' ? 'fa-exclamation-circle' : 'fa-exclamation-triangle'}"></i>
        <div>${message}</div>
    `;
    document.body.appendChild(alertDiv);
    
    setTimeout(() => {
        alertDiv.style.opacity = "0";
        alertDiv.style.transition = "opacity 0.5s ease";
        setTimeout(() => alertDiv.remove(), 500);
    }, 4000);
}
