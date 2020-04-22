cls

pushd %~dp0

call yarn
call yarn prod

popd