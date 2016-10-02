default: none

none:
	$(error "To publish the package, run `make publish`")

publish:
	rm *.nupkg
	nuget pack FossilDelta.nuspec
	nuget push *.nupkg
