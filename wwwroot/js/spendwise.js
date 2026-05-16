(() => {
  const SHOW_MS = 3000;
  const FADE_MS = 400;

  document.addEventListener('DOMContentLoaded', () => {
    const toast = document.querySelector('.toast-success');
    if (!toast) return;

    window.setTimeout(() => {
      toast.style.transition = `opacity ${FADE_MS}ms ease`;
      toast.style.opacity = '0';
      window.setTimeout(() => {
        try {
          toast.remove();
        } catch {
          /* phần tử đã bị gỡ khỏi DOM */
        }
      }, FADE_MS);
    }, SHOW_MS);
  });
})();
