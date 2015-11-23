package ru.ssau;

import org.opencv.core.Core;
import org.opencv.core.Mat;
import org.opencv.imgcodecs.Imgcodecs;
import ru.ssau.I18n.Strings;

public class App 
{
	static{ System.loadLibrary(Core.NATIVE_LIBRARY_NAME); }

	public static void main(String[] args) throws Exception {
		String filePath = "src/main/resources/images/marble.jpg";
		
		
		Mat newImage = Imgcodecs.imread(filePath);

		if(newImage.dataAddr()==0){
			System.out.println("Couldn't open file " + filePath);
		}else{

			GUI gui = new GUI(Strings.getAppTitle(), newImage);
			gui.init();
		}
		return;
	}
}