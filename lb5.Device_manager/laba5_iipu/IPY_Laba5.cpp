#include <opencv2\opencv.hpp>
#include <opencv2\highgui\highgui.hpp>
#include <iostream>
#include <ctime>
#include <windows.h>
#include <conio.h>
#include <stdio.h>
#define FOLDER_PATH "D:\\Captures\\"

#pragma comment(lib, "user32.lib")

using namespace std;
using namespace cv;

HWND hConsoleWindow;
VideoCapture vcap(0);

bool showWindowStatus = true;
bool fl_save_photo = false;
bool fl_record_video = false;

int photo_count = 0;
int video_count = 0;

template <typename T>
std::string toString(T val) {
	std::ostringstream oss;
	oss << val;
	return oss.str();
}

LRESULT CALLBACK keyboardProc(int nCode, WPARAM wParam, LPARAM lParam) {
	if(wParam == WM_KEYUP) {
		switch((char)((PKBDLLHOOKSTRUCT) lParam)->vkCode) {
			case 'H':
				showWindowStatus = !showWindowStatus;
				ShowWindow(hConsoleWindow, showWindowStatus);
				break;

			case 'C':
				fl_save_photo = true;
				break;

			case 'S':
				fl_record_video = true;
				break;

			case 'F':
				fl_record_video = false;
				break;
		}
	}

	return CallNextHookEx(NULL, nCode, wParam, lParam);
}

DWORD WINAPI MyKeyBoardLogger(LPVOID lpParm) {
	HHOOK hhkLowLevelKybd = SetWindowsHookEx(WH_KEYBOARD_LL, keyboardProc, 0, 0);
	MSG message;
	while (!GetMessage(&message, NULL, NULL, NULL)) {
		TranslateMessage(&message);
		DispatchMessage(&message);
	}
	UnhookWindowsHookEx(hhkLowLevelKybd);
	return 0;
}

void capturePhoto(Mat frame) {
	vector<int> compression_params;
	compression_params.push_back(CV_IMWRITE_JPEG_QUALITY);
	compression_params.push_back(100);

	String photo_name = "image" + toString(time(NULL)) + toString(photo_count++) + ".jpg";
	imwrite(FOLDER_PATH + photo_name, frame, compression_params);
	cout << photo_name + " - saved." << endl;

	fl_save_photo = false;
}

void recordVideo(Mat frame) { 
	int frame_width = vcap.get(CV_CAP_PROP_FRAME_WIDTH);
	int frame_height = vcap.get(CV_CAP_PROP_FRAME_HEIGHT);

	String video_name = "video" + toString(time(NULL)) + ".avi";
	VideoWriter video(FOLDER_PATH + video_name, CV_FOURCC('M', 'J', 'P', 'G'), 25, Size(frame_width, frame_height), true);

	cout << video_name + " - start record." << endl;
	while(fl_record_video) {
		vcap >> frame;
		if(fl_record_video) {
			video.write(frame);
		} else {
			video.release();
		}
		if(fl_save_photo) {
			capturePhoto(frame);
		}
	}
	cout << video_name + " - finish record and saved." << endl;
}

int main(int argc, char** argv) {
	hConsoleWindow = GetForegroundWindow();
	CreateThread(NULL, NULL, MyKeyBoardLogger, NULL, NULL, NULL);

	Mat frame;
	while (true) {
		vcap >> frame;
		if (fl_record_video) {
			recordVideo(frame);
		} else if(fl_save_photo) {
			capturePhoto(frame);
		}
	}
	return 0;
}