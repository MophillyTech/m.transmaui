bN=$(/usr/libexec/PlistBuddy -c "Print :CFBundleVersion" "Info.plist")

/usr/bin/zip -r bin/iPhone/Deploy_iPhone/mtransportiOS-dSYM-${bN}.zip bin/iPhone/Deploy_iPhone/mtransportiOS.app.dSYM

curl -F "dsym=@bin/iPhone/Deploy_iPhone/mtransportiOS-dSYM-${bN}.zip;type=application/zip" https://xaapi.xamarin.com/api/dsym?apikey=0be8d04ffa9b00ca5fb208211a1d870e28ee7bd2


