using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GameLib {
	internal class NativeMethods {

		internal static uint SRCINVERT = 0x00660046;

		[DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
		static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
		[DllImport("gdi32.dll")]
		static extern int DeleteObject(IntPtr hdc);
		[DllImport("gdi32.dll")]
		static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc, int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, uint dwRop );

		internal static void drawBmp(Graphics g, Bitmap bmp, int dstX, int dstY, int dstW, int dstH, int srcX, int srcY, int srcW, int srcH, uint operation) {
			IntPtr hDC = g.GetHdc();
			IntPtr hBmpSrc = bmp.GetHbitmap();
			using(Graphics gdraw = Graphics.FromImage(bmp) ) {
				IntPtr srcHDC = gdraw.GetHdc();
				IntPtr preHDC = SelectObject(srcHDC, hBmpSrc);
				StretchBlt(hDC, dstX,dstY, dstW,dstH, srcHDC, srcX,srcY, srcW,srcH, operation );
				SelectObject(preHDC, hBmpSrc);
				DeleteObject(hBmpSrc);
				g.ReleaseHdc(hDC);
				gdraw.ReleaseHdc(srcHDC);
			}
		}
		internal static void drawBmp(Graphics g, Bitmap bmp, Rectangle dst, Rectangle src, uint operation) {
			drawBmp(g, bmp, dst.X,dst.Y,dst.Width,dst.Height, src.X,src.Y,src.Width,src.Height, operation);
		}
	}
}
