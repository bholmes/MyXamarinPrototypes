XBUILD=/Applications/Xcode.app/Contents/Developer/usr/bin/xcodebuild
PROJECT_ROOT=./ObjCStuff/MatrixTestLib
PROJECT=$(PROJECT_ROOT)/MatrixTestLib.xcodeproj
TARGET=MatrixTestLib

all: libMatrixTestLibSDK.a

libMatrixTestLib-i386.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphonesimulator -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphonesimulator/lib$(TARGET).a $@

libMatrixTestLib-armv7.a:
	$(XBUILD) -project $(PROJECT) -target $(TARGET) -sdk iphoneos -arch armv7 -configuration Release clean build
	-mv $(PROJECT_ROOT)/build/Release-iphoneos/lib$(TARGET).a $@

libMatrixTestLibSDK.a: libMatrixTestLib-i386.a libMatrixTestLib-armv7.a
	lipo -create -output $@ $^

clean:
	-rm -rf *.a *.dll build
