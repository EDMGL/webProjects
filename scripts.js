let camera, scene, renderer;
// Global değişkenleri tanımlayalım
let isUserInteracting = false;
let onPointerDownMouseX = 0;
let onPointerDownMouseY = 0;
let lon = 0;
let onPointerDownLon = 0;
let lat = 0; // lat değişkenini buraya taşıdık
let onPointerDownLat = 0;
let phi = 0;
let theta = 0;

// Tüm yardımcı fonksiyonları önce tanımlayalım
function onDocumentMouseDown(event) {
    isUserInteracting = true;
    onPointerDownMouseX = event.clientX;
    onPointerDownMouseY = event.clientY;
    onPointerDownLon = lon;
    onPointerDownLat = lat;
}

function onDocumentMouseMove(event) {
    if (isUserInteracting) {
        lon = (onPointerDownMouseX - event.clientX) * 0.1 + onPointerDownLon;
        lat = (event.clientY - onPointerDownMouseY) * 0.1 + onPointerDownLat;
    }
}

function onDocumentMouseUp() {
    isUserInteracting = false;
}

function onDocumentWheel(event) {
    const fov = camera.fov + event.deltaY * 0.05;
    camera.fov = THREE.MathUtils.clamp(fov, 30, 90);
    camera.updateProjectionMatrix();
}

function onWindowResize() {
    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix();
    renderer.setSize(window.innerWidth, window.innerHeight);
}

function update() {
    if (!camera) return;
    
    lat = Math.max(-85, Math.min(85, lat));
    phi = THREE.MathUtils.degToRad(90 - lat);
    theta = THREE.MathUtils.degToRad(lon);
    
    camera.position.x = 100 * Math.sin(phi) * Math.cos(theta);
    camera.position.y = 100 * Math.cos(phi);
    camera.position.z = 100 * Math.sin(phi) * Math.sin(theta);
    
    camera.lookAt(scene.position);
    renderer.render(scene, camera);
}

function animate() {
    requestAnimationFrame(animate);
    update();
}

function init() {
    // Sahne oluştur
    scene = new THREE.Scene();
    
    // Kamera oluştur
    camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
    camera.position.set(0, 0, 0.1);
    
    // Küresel geometri oluştur
    const textureLoader = new THREE.TextureLoader();
    textureLoader.load(
        'images/resim.jpeg',
        function(texture) {
            const geometry = new THREE.SphereGeometry(500, 60, 40);
            geometry.scale(-1, 1, 1); // İç yüzeyi göster
            
            // Doku yükleyici
            const material = new THREE.MeshBasicMaterial({
                map: texture,
                side: THREE.DoubleSide
            });
            
            // Küre mesh'i oluştur
            const mesh = new THREE.Mesh(geometry, material);
            scene.add(mesh);
        },
        undefined,
        function(err) {
            console.error('Texture yüklenirken hata oluştu:', err);
        }
    );
    
    // Renderer oluştur
    renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(window.innerWidth, window.innerHeight);
    document.body.appendChild(renderer.domElement);
    
    // Mouse kontrollerini ekle
    document.addEventListener('mousedown', onDocumentMouseDown);
    document.addEventListener('mousemove', onDocumentMouseMove);
    document.addEventListener('mouseup', onDocumentMouseUp);
    document.addEventListener('wheel', onDocumentWheel);
    window.addEventListener('resize', onWindowResize, false);
}

// Uygulamayı başlat
init();
animate();
