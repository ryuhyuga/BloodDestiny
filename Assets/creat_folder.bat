@echo off
REM Tạo cấu trúc thư mục cho dự án Unity.
REM Chạy file .bat này trong thư mục Assets của dự án Unity.

echo Tạo thư mục BloodLotus...
mkdir BloodLotus
if errorlevel 1 (
    echo error: Cannot create BloodLotus.
    goto :end
)

echo Tạo thư mục Animations...
mkdir BloodLotus\Animations
mkdir BloodLotus\Animations\Player
mkdir BloodLotus\Animations\Enemies
mkdir BloodLotus\Animations\Weapons
if errorlevel 1 (
    echo error: Cannot create Animations.
    goto :end
)

echo Tạo thư mục Art...
mkdir BloodLotus\Art
mkdir BloodLotus\Art\Sprites
mkdir BloodLotus\Art\Tilemaps
mkdir BloodLotus\Art\VFX
if errorlevel 1 (
    echo error: Cannot create Art.
    goto :end
)

echo Tạo thư mục Audio...
mkdir BloodLotus\Audio
mkdir BloodLotus\Audio\Music
mkdir BloodLotus\Audio\SFX
if errorlevel 1 (
    echo error: Cannot create Audio.
    goto :end
)

echo Tạo thư mục DataAssets...
mkdir BloodLotus\DataAssets
mkdir BloodLotus\DataAssets\Weapons
mkdir BloodLotus\DataAssets\Skills
mkdir BloodLotus\DataAssets\InnerPowers
mkdir BloodLotus\DataAssets\Enemies
mkdir BloodLotus\DataAssets\Artifacts
mkdir BloodLotus\DataAssets\Bloodlines
mkdir BloodLotus\DataAssets\EstateUpgrades
mkdir BloodLotus\DataAssets\Levels
if errorlevel 1 (
    echo error: Cannot create DataAssets.
    goto :end
)

echo Tạo thư mục Prefabs...
mkdir BloodLotus\Prefabs
mkdir BloodLotus\Prefabs\Characters
mkdir BloodLotus\Prefabs\Enemies
mkdir BloodLotus\Prefabs\Items
mkdir BloodLotus\Prefabs\Projectiles
mkdir BloodLotus\Prefabs\UI
if errorlevel 1 (
    echo error: Cannot create Prefabs.
    goto :end
)

echo Tạo thư mục Scenes...
mkdir BloodLotus\Scenes
if errorlevel 1 (
    echo error: Cannot create Scenes.
    goto :end
)

echo Tạo thư mục Scripts...
mkdir BloodLotus\Scripts
mkdir BloodLotus\Scripts\Core
mkdir BloodLotus\Scripts\Components
mkdir BloodLotus\Scripts\Data
mkdir BloodLotus\Scripts\Systems
mkdir BloodLotus\Scripts\Input
mkdir BloodLotus\Scripts\UI
mkdir BloodLotus\Scripts\Animation
mkdir BloodLotus\Scripts\Editor
if errorlevel 1 (
    echo error: Cannot create Scripts.
    goto :end
)

echo Tạo thư mục Settings...
mkdir BloodLotus\Settings
if errorlevel 1 (
    echo error: Cannot create Settings.
    goto :end
)

echo Hoàn tất tạo cấu trúc thư mục.
pause

:end
exit